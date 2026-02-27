using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using TNRD.Utilities;
using UnityEditor;
#endif

public class WebcamToModelBinder : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private WebCam webCam;
    [SerializeField] private float positionScaleXY = 0.25f;
    [SerializeField] private GameObject student;
    [SerializeField] private GameObject teacher;
    [SerializeField] private float smoothing = 0.5f;
    [SerializeField] private bool autoAlign = true;

    [Header("Head")]
    [SerializeField] private float headYawSensitivity = 25f;
    [SerializeField] private float headPitchSensitivity = 15f;
    [SerializeField] private float headZCompensation = 1f;
    [SerializeField] private Vector3 defaultHeadRotationOffset = Vector3.zero;
    [SerializeField] private Vector3 minHeadDeviation = new Vector3(-45f, -60f, -30f);
    [SerializeField] private Vector3 maxHeadDeviation = new Vector3(45f, 60f, 30f);
    [SerializeField] private float headExtraSmoothing = 0.7f;
    [SerializeField] private float headYawDeadZone = 0.5f;
    [SerializeField] private float headPitchDeadZone = 0.5f;
    [SerializeField] private float headLagTime = 0.5f;

    [Header("Arm IK")]
    [SerializeField] private float maxArmStretch = 1.05f;

    [Header("Leg IK")]
    [SerializeField] private float maxLegStretch = 1.05f;
    [SerializeField] private float minLegStretchRatio = 0.3f;
    [SerializeField] private float legPoleStrength = 1f;
    [SerializeField] private Vector3 legPreferredBendDirection = Vector3.forward;
    [SerializeField] private float minKneeAngle = 60f;

    [Header("Hand Orientation")]
    [SerializeField] private Vector3 leftHandDefaultRotationOffset = Vector3.zero;
    [SerializeField] private Vector3 rightHandDefaultRotationOffset = Vector3.zero;

    [Header("Matching Mechanics")]
    [SerializeField] private int numRepetitions = 1;
#if UNITY_EDITOR
    [SerializeField] private Vector2 accuracyOffset = new Vector2(0f, 2.2f); // Offset for accuracy percentage text above student
    [SerializeField] private Vector2 instructionOffset = new Vector2(0f, 2.7f); // Offset for instruction/countdown text above student
#endif
    [SerializeField] private float matchTolerance = 0.15f; // Maximum allowed distance (meters) for a point to be considered matching
    [SerializeField] private float maxAccuracyDropPerSecond = 20f;
    [SerializeField] private float startDelaySeconds = 3f; // How long the player must hold the starting pose
    [SerializeField] private float requiredStartAccuracy = 90f; // Minimum accuracy to start countdown
    [SerializeField] private int repetitionsCount = 1;
    private float debugTimer = -1f;
    private List<string> debugSnapshots = new List<string>();
    private float debugCollectionTimer = 0f;
    private float smoothedAccuracy = 0f;
    private int currentLoop = 0;
    private float startFlashTimer = 0f;
    private bool showStartFlash = false;
    private float lastRepAccuracy = 0f;               // точность предыдущего повтора
    private List<float> repAverages = new List<float>();

    private GameObject[] humans;
    private Animator[] animators;
    private Transform[] bUpperArmLs, bForearmLs, bHandLs;
    private Transform[] bUpperArmRs, bForearmRs, bHandRs;
    private Transform[] bThighLs, bShinLs, bFootLs;
    private Transform[] bThighRs, bShinRs, bFootRs;
    private Transform[] bHeads;
    private Quaternion[] initialUpperArmLRots, initialForearmLRots, initialHandLRots;
    private Quaternion[] initialUpperArmRRots, initialForearmRRots, initialHandRRots;
    private Quaternion[] initialThighLRots, initialShinLRots, initialFootLRots;
    private Quaternion[] initialThighRRots, initialShinRRots, initialFootRRots;
    private Quaternion[] initialHeadRots;
    private Vector3[] initUpperArmLLocals, initForearmLLocals, initHandLLocals;
    private Vector3[] initUpperArmRLocals, initForearmRLocals, initHandRLocals;
    private Vector3[] initThighLLocals, initShinLLocals, initFootLLocals;
    private Vector3[] initThighRLocals, initShinRLocals, initFootRLocals;
    private Vector3[] initHeadLocals;
    private float[] upperArmLengthLs, forearmLengthLs;
    private float[] upperArmLengthRs, forearmLengthRs;
    private float[] thighLengthLs, shinLengthLs;
    private float[] thighLengthRs, shinLengthRs;
    private Vector3[] initUpperArmLDirs, initForearmLDirs;
    private Vector3[] initUpperArmRDirs, initForearmRDirs;
    private Vector3[] initThighLDirs, initShinLDirs;
    private Vector3[] initThighRDirs, initShinRDirs;
    private Vector3[] initLeftShoulderPoss, initRightShoulderPoss;
    private Vector3[] initLeftHipPoss, initRightHipPoss;
    private Vector3[] initHeadPoss;
    private Vector3[][] smoothedTargets;
    private Vector3[] initialHumanPositions;
    private float[] smoothedYaws;
    private float[] smoothedPitches;
    private Quaternion[] relativeHandLRots, relativeHandRRots;

    private bool isLogging = false;
    private bool isRecording = false;
    private bool isPlayback = false;
    private int playbackIndex = 0;
    private float playbackTimer = 0f;
    private float recordInterval = 0.1f;

    private enum MatchingPhase { Standby, Countdown, Active }
    private MatchingPhase matchingPhase = MatchingPhase.Standby;
    private float countdownTimer = 0f;
    private float currentAccuracy = 0f;
    private List<float> sessionAccuracies = new List<float>();
    private string accuracyText = "";
    private Color accuracyColor = Color.white;
    private string instructionText = "";
    private Color instructionColor = Color.white;

    [System.Serializable]
    private class PoseFrame
    {
        public Vector3[] positions;
        public float zValue;
    }

    private List<PoseFrame> recordedFrames = new List<PoseFrame>();
    private float recordTimer = 0f;

#if UNITY_EDITOR
    private GameObject[] gizmoObjs = new GameObject[13];
#endif

    private void Start()
    {
        humans = new GameObject[] { student, teacher };
        if (humans[0] == null && humans[1] == null) return;
        int n = humans.Length;
        animators = new Animator[n];
        bUpperArmLs = new Transform[n]; bForearmLs = new Transform[n]; bHandLs = new Transform[n];
        bUpperArmRs = new Transform[n]; bForearmRs = new Transform[n]; bHandRs = new Transform[n];
        bThighLs = new Transform[n]; bShinLs = new Transform[n]; bFootLs = new Transform[n];
        bThighRs = new Transform[n]; bShinRs = new Transform[n]; bFootRs = new Transform[n];
        bHeads = new Transform[n];
        initialUpperArmLRots = new Quaternion[n]; initialForearmLRots = new Quaternion[n]; initialHandLRots = new Quaternion[n];
        initialUpperArmRRots = new Quaternion[n]; initialForearmRRots = new Quaternion[n]; initialHandRRots = new Quaternion[n];
        initialThighLRots = new Quaternion[n]; initialShinLRots = new Quaternion[n]; initialFootLRots = new Quaternion[n];
        initialThighRRots = new Quaternion[n]; initialShinRRots = new Quaternion[n]; initialFootRRots = new Quaternion[n];
        initialHeadRots = new Quaternion[n];
        initUpperArmLLocals = new Vector3[n]; initForearmLLocals = new Vector3[n]; initHandLLocals = new Vector3[n];
        initUpperArmRLocals = new Vector3[n]; initForearmRLocals = new Vector3[n]; initHandRLocals = new Vector3[n];
        initThighLLocals = new Vector3[n]; initShinLLocals = new Vector3[n]; initFootLLocals = new Vector3[n];
        initThighRLocals = new Vector3[n]; initShinRLocals = new Vector3[n]; initFootRLocals = new Vector3[n];
        initHeadLocals = new Vector3[n];
        upperArmLengthLs = new float[n]; forearmLengthLs = new float[n];
        upperArmLengthRs = new float[n]; forearmLengthRs = new float[n];
        thighLengthLs = new float[n]; shinLengthLs = new float[n];
        thighLengthRs = new float[n]; shinLengthRs = new float[n];
        initUpperArmLDirs = new Vector3[n]; initForearmLDirs = new Vector3[n];
        initUpperArmRDirs = new Vector3[n]; initForearmRDirs = new Vector3[n];
        initThighLDirs = new Vector3[n]; initShinLDirs = new Vector3[n];
        initThighRDirs = new Vector3[n]; initShinRDirs = new Vector3[n];
        initLeftShoulderPoss = new Vector3[n]; initRightShoulderPoss = new Vector3[n];
        initLeftHipPoss = new Vector3[n]; initRightHipPoss = new Vector3[n];
        initHeadPoss = new Vector3[n];
        initialHumanPositions = new Vector3[n];
        smoothedTargets = new Vector3[n][];
        smoothedYaws = new float[n];
        smoothedPitches = new float[n];
        relativeHandLRots = new Quaternion[n];
        relativeHandRRots = new Quaternion[n];
        for (int j = 0; j < n; j++)
        {
            smoothedTargets[j] = new Vector3[13];
        }
#if UNITY_EDITOR
        for (int i = 0; i < 13; i++)
        {
            gizmoObjs[i] = new GameObject("TargetPoint_" + i);
            ShapeIcon icon = ShapeIcon.CircleGray;
            if (i == 0) icon = ShapeIcon.CircleGray;
            if (i >= 1 && i <= 3) icon = ShapeIcon.CircleRed;
            if (i >= 4 && i <= 6) icon = ShapeIcon.CircleBlue;
            if (i >= 7 && i <= 9) icon = ShapeIcon.CircleYellow;
            if (i >= 10 && i <= 12) icon = ShapeIcon.CircleGreen;
            IconManager.SetIcon(gizmoObjs[i], icon);
        }
#endif
        for (int j = 0; j < n; j++)
        {
            GameObject h = humans[j];
            if (h == null) continue;
            initialHumanPositions[j] = h.transform.position;
            animators[j] = h.GetComponent<Animator>();
            if (animators[j] != null) animators[j].enabled = false;
            Transform rig = h.transform.Find("Rig");
            if (rig == null) { Debug2.LogWarning($"Missing Rig in {h.name}"); continue; }
            Transform bRoot = rig.Find("B-root");
            Transform bHips = bRoot?.Find("B-hips");
            Transform bSpine = bHips?.Find("B-spine");
            Transform bChest = bSpine?.Find("B-chest");
            Transform bNeck = bChest?.Find("B-neck");
            bHeads[j] = bNeck?.Find("B-head");
            Transform bShoulderL = bChest?.Find("B-shoulder.L");
            bUpperArmLs[j] = bShoulderL?.Find("B-upperArm.L");
            bForearmLs[j] = bUpperArmLs[j]?.Find("B-forearm.L");
            bHandLs[j] = bForearmLs[j]?.Find("B-hand.L");
            Transform bShoulderR = bChest?.Find("B-shoulder.R");
            bUpperArmRs[j] = bShoulderR?.Find("B-upperArm.R");
            bForearmRs[j] = bUpperArmRs[j]?.Find("B-forearm.R");
            bHandRs[j] = bForearmRs[j]?.Find("B-hand.R");
            bThighLs[j] = bHips?.Find("B-thigh.L");
            bShinLs[j] = bThighLs[j]?.Find("B-shin.L");
            bFootLs[j] = bShinLs[j]?.Find("B-foot.L");
            bThighRs[j] = bHips?.Find("B-thigh.R");
            bShinRs[j] = bThighRs[j]?.Find("B-shin.R");
            bFootRs[j] = bShinRs[j]?.Find("B-foot.R");
            bool missingBone = bHeads[j] == null || bUpperArmLs[j] == null || bForearmLs[j] == null || bHandLs[j] == null ||
                               bUpperArmRs[j] == null || bForearmRs[j] == null || bHandRs[j] == null ||
                               bThighLs[j] == null || bShinLs[j] == null || bFootLs[j] == null ||
                               bThighRs[j] == null || bShinRs[j] == null || bFootRs[j] == null;
            if (missingBone)
            {
                Debug2.LogWarning($"Model {h.name} is missing some required bones.");
                continue;
            }
            initialUpperArmLRots[j] = bUpperArmLs[j].rotation; initialForearmLRots[j] = bForearmLs[j].rotation; initialHandLRots[j] = bHandLs[j].rotation;
            initialUpperArmRRots[j] = bUpperArmRs[j].rotation; initialForearmRRots[j] = bForearmRs[j].rotation; initialHandRRots[j] = bHandRs[j].rotation;
            initialThighLRots[j] = bThighLs[j].rotation; initialShinLRots[j] = bShinLs[j].rotation; initialFootLRots[j] = bFootLs[j].rotation;
            initialThighRRots[j] = bThighRs[j].rotation; initialShinRRots[j] = bShinRs[j].rotation; initialFootRRots[j] = bFootRs[j].rotation;
            initialHeadRots[j] = bHeads[j].rotation;
            initUpperArmLLocals[j] = bUpperArmLs[j].localPosition; initForearmLLocals[j] = bForearmLs[j].localPosition; initHandLLocals[j] = bHandLs[j].localPosition;
            initUpperArmRLocals[j] = bUpperArmRs[j].localPosition; initForearmRLocals[j] = bForearmRs[j].localPosition; initHandRLocals[j] = bHandRs[j].localPosition;
            initThighLLocals[j] = bThighLs[j].localPosition; initShinLLocals[j] = bShinLs[j].localPosition; initFootLLocals[j] = bFootLs[j].localPosition;
            initThighRLocals[j] = bThighRs[j].localPosition; initShinRLocals[j] = bShinRs[j].localPosition; initFootRLocals[j] = bFootRs[j].localPosition;
            initHeadLocals[j] = bHeads[j].localPosition;
            upperArmLengthLs[j] = Vector3.Distance(bUpperArmLs[j].position, bForearmLs[j].position);
            forearmLengthLs[j] = Vector3.Distance(bForearmLs[j].position, bHandLs[j].position);
            upperArmLengthRs[j] = Vector3.Distance(bUpperArmRs[j].position, bForearmRs[j].position);
            forearmLengthRs[j] = Vector3.Distance(bForearmRs[j].position, bHandRs[j].position);
            thighLengthLs[j] = Vector3.Distance(bThighLs[j].position, bShinLs[j].position);
            shinLengthLs[j] = Vector3.Distance(bShinLs[j].position, bFootLs[j].position);
            thighLengthRs[j] = Vector3.Distance(bThighRs[j].position, bShinRs[j].position);
            shinLengthRs[j] = Vector3.Distance(bShinRs[j].position, bFootRs[j].position);
            initUpperArmLDirs[j] = (bForearmLs[j].position - bUpperArmLs[j].position).normalized;
            initForearmLDirs[j] = (bHandLs[j].position - bForearmLs[j].position).normalized;
            initUpperArmRDirs[j] = (bForearmRs[j].position - bUpperArmRs[j].position).normalized;
            initForearmRDirs[j] = (bHandRs[j].position - bForearmRs[j].position).normalized;
            initThighLDirs[j] = (bShinLs[j].position - bThighLs[j].position).normalized;
            initShinLDirs[j] = (bFootLs[j].position - bShinLs[j].position).normalized;
            initThighRDirs[j] = (bShinRs[j].position - bThighRs[j].position).normalized;
            initShinRDirs[j] = (bFootRs[j].position - bShinRs[j].position).normalized;
            initLeftShoulderPoss[j] = bUpperArmLs[j].position;
            initRightShoulderPoss[j] = bUpperArmRs[j].position;
            initLeftHipPoss[j] = bThighLs[j].position;
            initRightHipPoss[j] = bThighRs[j].position;
            initHeadPoss[j] = bHeads[j].position;
            relativeHandLRots[j] = Quaternion.Inverse(initialForearmLRots[j]) * initialHandLRots[j];
            relativeHandRRots[j] = Quaternion.Inverse(initialForearmRRots[j]) * initialHandRRots[j];
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && !isLogging)
        {
            StartCoroutine(LogPositionsOverTime());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isPlayback = false;
            isRecording = true;
            recordedFrames.Clear();
            recordTimer = 0f;
            Debug2.Log("Recording started");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            isRecording = false;
            int delayFrames = Mathf.RoundToInt(startDelaySeconds / recordInterval);
            if (recordedFrames.Count > delayFrames)
            {
                recordedFrames.RemoveRange(0, delayFrames);
            }
            SaveRecording();
            Debug2.Log("Recording stopped");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isRecording = false;
            isPlayback = !isPlayback;

            if (isPlayback)
            {
                playbackIndex = 0;
                playbackTimer = 0f;
                matchingPhase = MatchingPhase.Standby;
                countdownTimer = 0f;
                currentAccuracy = 0f;
                sessionAccuracies.Clear();
                repAverages.Clear(); // очищаем для нового запуска
                instructionText = "Stand in starting position";
                instructionColor = Color.yellow;
                accuracyText = "";
                currentLoop = 0;
                if (recordedFrames.Count == 0)
                    LoadRecording();
                debugTimer = 5f;
                debugSnapshots.Clear();
                debugCollectionTimer = 1f;
            }
            else
            {
                accuracyText = "";
                instructionText = "";
                debugTimer = -1f;
            }

            Debug2.Log("Playback mode " + (isPlayback ? "enabled" : "disabled"));
        }
    }

    private void LateUpdate()
    {
        if (webCam == null || (student == null && teacher == null)) return;
        if (webCam.AnnotationRoot == null) return;

        Vector3[] currentLocalPositions = webCam.GetAllPointPositions();
        float currentZValue = webCam.ZValue;

        if (currentLocalPositions == null)
        {
            ResetModel(0);
            ResetModel(1);
            return;
        }

        Transform annRoot = webCam.AnnotationRoot.transform;

        if (isRecording)
        {
            recordTimer += Time.deltaTime;
            if (recordTimer >= recordInterval)
            {
                recordTimer -= recordInterval;
                recordedFrames.Add(new PoseFrame
                {
                    positions = (Vector3[])currentLocalPositions.Clone(),
                    zValue = currentZValue
                });
            }
            ResetModel(0);
            ApplyPose(1, currentLocalPositions, currentZValue, annRoot);
        }
        else if (isPlayback)
        {
            ApplyPose(0, currentLocalPositions, currentZValue, annRoot);

            if (matchingPhase == MatchingPhase.Active)
            {
                playbackTimer += Time.deltaTime;
                if (playbackTimer >= recordInterval)
                {
                    playbackTimer -= recordInterval;
                    playbackIndex++;
                }
            }

            if (playbackIndex >= recordedFrames.Count)
            {
                // Считаем среднюю по завершённому повтору
                if (sessionAccuracies.Count > 0)
                {
                    float repSum = 0f;
                    foreach (float a in sessionAccuracies) repSum += a;
                    float repAvg = repSum / sessionAccuracies.Count;
                    lastRepAccuracy = repAvg;
                    repAverages.Add(repAvg);
                }

                currentLoop++;

                if (currentLoop < repetitionsCount)
                {
                    playbackIndex = 0;
                    playbackTimer = 0f;
                    matchingPhase = MatchingPhase.Active;
                    sessionAccuracies.Clear();
                }
                else
                {
                    isPlayback = false;

                    // Один чистый лог
                    if (repAverages.Count > 0)
                    {
                        StringBuilder logBuilder = new StringBuilder();
                        for (int i = 0; i < repAverages.Count; i++)
                        {
                            logBuilder.AppendLine($"Rep {i + 1}/{repetitionsCount}: {repAverages[i]:F1}%");
                        }

                        float totalSum = 0f;
                        foreach (float avg in repAverages) totalSum += avg;
                        float totalAvg = totalSum / repAverages.Count;
                        logBuilder.AppendLine($"Overall average: {totalAvg:F1}%");

                        Debug2.Log(logBuilder.ToString());
                    }

                    accuracyText = "";
                    instructionText = "Finished";
                    instructionColor = Color.cyan;
                    ResetModel(1);
                    debugTimer = -1f;

                    repAverages.Clear();
                    return;
                }
            }

            PoseFrame currentFrame = recordedFrames[playbackIndex];
            Vector3[] interpPositions = (Vector3[])currentFrame.positions.Clone();
            float interpZ = currentFrame.zValue;

            if (playbackIndex + 1 < recordedFrames.Count && matchingPhase == MatchingPhase.Active)
            {
                float t = playbackTimer / recordInterval;
                PoseFrame nextFrame = recordedFrames[playbackIndex + 1];
                for (int i = 0; i < 13; i++)
                {
                    interpPositions[i] = Vector3.Lerp(currentFrame.positions[i], nextFrame.positions[i], t);
                }
                interpZ = Mathf.Lerp(currentFrame.zValue, nextFrame.zValue, t);
            }

            ApplyPose(1, interpPositions, interpZ, annRoot);

            if (debugTimer > 0f)
            {
                if (debugCollectionTimer >= 1f)
                {
                    debugSnapshots.Add(BuildDebugSnapshot());
                    debugCollectionTimer -= 1f;
                }
                debugTimer -= Time.deltaTime;
                debugCollectionTimer += Time.deltaTime;

                if (debugTimer <= 0f)
                {
                    LogDebugPositions();
                }
            }

            Vector3 center0 = (smoothedTargets[0][1] + smoothedTargets[0][4] + smoothedTargets[0][7] + smoothedTargets[0][10]) / 4f;
            Vector3 center1 = (smoothedTargets[1][1] + smoothedTargets[1][4] + smoothedTargets[1][7] + smoothedTargets[1][10]) / 4f;

            Vector3 shoulder_vec0 = smoothedTargets[0][4] - smoothedTargets[0][1];
            Vector3 shoulder_vec1 = smoothedTargets[1][4] - smoothedTargets[1][1];
            Quaternion alignment_rot = Quaternion.FromToRotation(shoulder_vec0.normalized, shoulder_vec1.normalized);

            Vector3 mid_shoulder0 = (smoothedTargets[0][1] + smoothedTargets[0][4]) / 2f;
            Vector3 mid_hip0 = (smoothedTargets[0][7] + smoothedTargets[0][10]) / 2f;
            float torso_length0 = Vector3.Distance(mid_shoulder0, mid_hip0);

            Vector3 mid_shoulder1 = (smoothedTargets[1][1] + smoothedTargets[1][4]) / 2f;
            Vector3 mid_hip1 = (smoothedTargets[1][7] + smoothedTargets[1][10]) / 2f;
            float torso_length1 = Vector3.Distance(mid_shoulder1, mid_hip1);

            float scale_factor = (torso_length0 > 0f && torso_length1 > 0f) ? torso_length1 / torso_length0 : 1f;

            float acc = 0f;
            for (int i = 0; i < 13; i++)
            {
                Vector3 pos0_centered = smoothedTargets[0][i] - center0;
                Vector3 pos0_rotated = alignment_rot * pos0_centered;
                Vector3 pos0 = pos0_rotated * scale_factor;
                Vector3 pos1 = smoothedTargets[1][i] - center1;
                float dist = Vector3.Distance(pos0, pos1);
                acc += Mathf.Clamp01(1f - (dist / matchTolerance));
            }
            currentAccuracy = (acc / 13f) * 100f;

            float maxDropThisFrame = maxAccuracyDropPerSecond * Time.deltaTime;
            float newSmoothed = Mathf.Lerp(smoothedAccuracy, currentAccuracy, smoothing);
            if (newSmoothed < smoothedAccuracy - maxDropThisFrame)
            {
                newSmoothed = smoothedAccuracy - maxDropThisFrame;
            }
            smoothedAccuracy = newSmoothed;

            accuracyText = Mathf.RoundToInt(smoothedAccuracy).ToString() + "%";
            accuracyColor = Color.Lerp(Color.red, Color.green, smoothedAccuracy / 100f);

            if (matchingPhase == MatchingPhase.Standby)
            {
                instructionText = "Stand in starting position";
                instructionColor = Color.yellow;

                if (currentAccuracy >= requiredStartAccuracy)
                {
                    matchingPhase = MatchingPhase.Countdown;
                    countdownTimer = startDelaySeconds;
                    smoothedAccuracy = currentAccuracy;
                }
            }
            else if (matchingPhase == MatchingPhase.Countdown)
            {
                countdownTimer -= Time.deltaTime;
                instructionText = Mathf.CeilToInt(countdownTimer).ToString("F0");
                float progress = (startDelaySeconds - countdownTimer) / startDelaySeconds;
                instructionColor = Color.Lerp(Color.yellow, Color.green, progress);

                if (countdownTimer <= 0f)
                {
                    matchingPhase = MatchingPhase.Active;
                    instructionText = "START!";
                    instructionColor = Color.green;
                    startFlashTimer = 2f;
                    showStartFlash = true;
                    sessionAccuracies.Clear();
                    smoothedAccuracy = currentAccuracy;
                }
                else if (currentAccuracy < requiredStartAccuracy - 5f)
                {
                    matchingPhase = MatchingPhase.Standby;
                    countdownTimer = 0f;
                    instructionText = "Stand in starting position";
                    instructionColor = Color.red;
                }
            }
            else if (matchingPhase == MatchingPhase.Active)
            {
                if (showStartFlash)
                {
                    startFlashTimer -= Time.deltaTime;
                    if (startFlashTimer <= 0f)
                    {
                        showStartFlash = false;
                    }
                }

                string repInfo = $"Rep {currentLoop + 1}/{repetitionsCount}";
                string accInfo = (currentLoop > 0) ? $" Last: {Mathf.RoundToInt(lastRepAccuracy)}%" : "";
                instructionText = repInfo + accInfo;
                instructionColor = Color.cyan;

                sessionAccuracies.Add(smoothedAccuracy);
            }
        }
        else
        {
            ApplyPose(0, currentLocalPositions, currentZValue, annRoot);
            ResetModel(1);
        }

#if UNITY_EDITOR
        for (int i = 0; i < 13; i++)
            gizmoObjs[i].transform.position = smoothedTargets[0][i];
#endif
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (student == null || !isPlayback) return;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 28;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        if (accuracyText != "")
        {
            Vector3 labelPos = student.transform.position + new Vector3(accuracyOffset.x, accuracyOffset.y, 0f);
            style.normal.textColor = accuracyColor;
            Handles.Label(labelPos, accuracyText, style);
        }

        if (instructionText != "")
        {
            Vector3 labelPos = student.transform.position + new Vector3(instructionOffset.x, instructionOffset.y, 0f);
            style.normal.textColor = instructionColor;
            Handles.Label(labelPos, instructionText, style);
        }
    }
#endif

    private IEnumerator LogPositionsOverTime()
    {
        isLogging = true;
        StringBuilder fullLog = new StringBuilder("Logged Positions Over Time:\n");
        for (int frame = 0; frame < 10; frame++)
        {
            StringBuilder sb = new StringBuilder($"Frame {frame}:\n");
            Transform annRoot = webCam.AnnotationRoot.transform;
            Vector3[] localPositions = webCam.GetAllPointPositions();
            if (localPositions != null)
            {
                for (int i = 0; i < 13; i++)
                {
                    Vector3 worldPos = annRoot.TransformPoint(localPositions[i]);
                    sb.AppendLine($"Point {i}: {worldPos}");
                }
            }
            fullLog.Append(sb.ToString());
            yield return new WaitForSeconds(0.5f);
        }
        Debug2.Log(fullLog.ToString());
        isLogging = false;
    }

    private void ResetModel(int j)
    {
        bUpperArmLs[j].localPosition = initUpperArmLLocals[j]; bForearmLs[j].localPosition = initForearmLLocals[j]; bHandLs[j].localPosition = initHandLLocals[j];
        bUpperArmRs[j].localPosition = initUpperArmRLocals[j]; bForearmRs[j].localPosition = initForearmRLocals[j]; bHandRs[j].localPosition = initHandRLocals[j];
        bThighLs[j].localPosition = initThighLLocals[j]; bShinLs[j].localPosition = initShinLLocals[j]; bFootLs[j].localPosition = initFootLLocals[j];
        bThighRs[j].localPosition = initThighRLocals[j]; bShinRs[j].localPosition = initShinRLocals[j]; bFootRs[j].localPosition = initFootRLocals[j];
        bHeads[j].localPosition = initHeadLocals[j];
        bUpperArmLs[j].rotation = initialUpperArmLRots[j]; bForearmLs[j].rotation = initialForearmLRots[j]; bHandLs[j].rotation = initialHandLRots[j];
        bUpperArmRs[j].rotation = initialUpperArmRRots[j]; bForearmRs[j].rotation = initialForearmRRots[j]; bHandRs[j].rotation = initialHandRRots[j];
        bThighLs[j].rotation = initialThighLRots[j]; bShinLs[j].rotation = initialShinLRots[j]; bFootLs[j].rotation = initialFootLRots[j];
        bThighRs[j].rotation = initialThighRRots[j]; bShinRs[j].rotation = initialShinRRots[j]; bFootRs[j].rotation = initialFootRRots[j];
        bHeads[j].rotation = Quaternion.Euler(defaultHeadRotationOffset) * initialHeadRots[j];
        humans[j].transform.position = initialHumanPositions[j];

        smoothedYaws[j] = 0f;
        smoothedPitches[j] = 0f;
        for (int i = 0; i < 13; i++)
        {
            smoothedTargets[j][i] = Vector3.zero;
        }
    }

    private void ApplyPose(int j, Vector3[] localPositions, float currentZ, Transform annRoot)
    {
        Vector3[] worldPositions = new Vector3[13];
        for (int i = 0; i < 13; i++) worldPositions[i] = annRoot.TransformPoint(localPositions[i]);
        Vector3 bodyCenter = (worldPositions[4] + worldPositions[1] + worldPositions[10] + worldPositions[7]) / 4f;
        humans[j].transform.position = initialHumanPositions[j];
        Vector3 modelPos = humans[j].transform.position;
        for (int i = 0; i < 13; i++)
        {
            Vector3 relative = (worldPositions[i] - bodyCenter) * positionScaleXY;
            Vector3 calcTarget = modelPos + relative;
            smoothedTargets[j][i] = Vector3.Lerp(smoothedTargets[j][i], calcTarget, smoothing);
        }
        bHeads[j].position = initHeadPoss[j];
        Vector3 nose = smoothedTargets[j][0];
        Vector3 inputShoulderMid = (smoothedTargets[j][1] + smoothedTargets[j][4]) / 2f;
        float ratio = currentZ / webCam.ReferenceZ;
        float depthMultiplier = Mathf.Pow(ratio, headZCompensation);
        float rawYaw = (inputShoulderMid.x - nose.x) * headYawSensitivity * depthMultiplier;
        float rawPitch = (nose.y - inputShoulderMid.y) * headPitchSensitivity * depthMultiplier;
        float yawDelta = rawYaw - smoothedYaws[j];
        if (Mathf.Abs(yawDelta) < headYawDeadZone) yawDelta = 0f;
        float pitchDelta = rawPitch - smoothedPitches[j];
        if (Mathf.Abs(pitchDelta) < headPitchDeadZone) pitchDelta = 0f;
        float alpha = 1f - Mathf.Exp(-Time.deltaTime / headLagTime);
        alpha *= headExtraSmoothing;
        smoothedYaws[j] += yawDelta * alpha;
        smoothedPitches[j] += pitchDelta * alpha;
        float clampedPitch = Mathf.Clamp(smoothedPitches[j], minHeadDeviation.x, maxHeadDeviation.x);
        float clampedYaw = Mathf.Clamp(smoothedYaws[j], minHeadDeviation.y, maxHeadDeviation.y);
        float clampedRoll = Mathf.Clamp(0f, minHeadDeviation.z, maxHeadDeviation.z);
        Vector3 totalEuler = defaultHeadRotationOffset + new Vector3(clampedPitch, clampedYaw, clampedRoll);
        bHeads[j].rotation = Quaternion.Euler(totalEuler) * initialHeadRots[j];
        if (autoAlign)
        {
            Vector3 targetCenter = (smoothedTargets[j][1] + smoothedTargets[j][4] + smoothedTargets[j][7] + smoothedTargets[j][10]) / 4f;
            Vector3 modelCenter = (bUpperArmLs[j].position + bUpperArmRs[j].position + bThighLs[j].position + bThighRs[j].position) / 4f;
            Vector3 offset = modelCenter - targetCenter;
            offset.z = 0f;
            for (int i = 0; i < 13; i++) smoothedTargets[j][i] += offset;
        }
        // Left arm
        Vector3 leftShoulderRoot = initLeftShoulderPoss[j];
        Vector3 leftElbowTarget = smoothedTargets[j][2];
        Vector3 leftWristTarget = smoothedTargets[j][3];
        ApplyArmIK(leftShoulderRoot, leftElbowTarget, leftWristTarget,
                   upperArmLengthLs[j], forearmLengthLs[j],
                   initUpperArmLDirs[j], initForearmLDirs[j],
                   initialUpperArmLRots[j], initialForearmLRots[j],
                   bUpperArmLs[j], bForearmLs[j], bHandLs[j],
                   relativeHandLRots[j], leftHandDefaultRotationOffset,
                   initForearmLLocals[j], initHandLLocals[j]);
        // Right arm
        Vector3 rightShoulderRoot = initRightShoulderPoss[j];
        Vector3 rightElbowTarget = smoothedTargets[j][5];
        Vector3 rightWristTarget = smoothedTargets[j][6];
        ApplyArmIK(rightShoulderRoot, rightElbowTarget, rightWristTarget,
                   upperArmLengthRs[j], forearmLengthRs[j],
                   initUpperArmRDirs[j], initForearmRDirs[j],
                   initialUpperArmRRots[j], initialForearmRRots[j],
                   bUpperArmRs[j], bForearmRs[j], bHandRs[j],
                   relativeHandRRots[j], rightHandDefaultRotationOffset,
                   initForearmRLocals[j], initHandRLocals[j]);
        // Left leg
        Vector3 leftHipRoot = initLeftHipPoss[j];
        Vector3 leftAnkleTarget = smoothedTargets[j][9];
        Vector3 leftKneeHint = smoothedTargets[j][8];
        Vector3 effectiveLeftAnkle;
        Vector3 kneeL = SolveTwoBoneIK(leftHipRoot, leftAnkleTarget, leftKneeHint, thighLengthLs[j], shinLengthLs[j], legPoleStrength, out effectiveLeftAnkle, legPreferredBendDirection);
        Vector3 toKnee = kneeL - leftHipRoot;
        float kneeDist = toKnee.magnitude;
        float minThigh = thighLengthLs[j] * minLegStretchRatio;
        float maxThigh = thighLengthLs[j] * maxLegStretch;
        if (kneeDist > maxThigh) { kneeL = leftHipRoot + toKnee.normalized * maxThigh; kneeDist = maxThigh; }
        else if (kneeDist < minThigh) { kneeL = leftHipRoot + toKnee.normalized * minThigh; kneeDist = minThigh; }
        Vector3 thighLDir = (kneeL - leftHipRoot).normalized;
        bThighLs[j].rotation = Quaternion.FromToRotation(initThighLDirs[j], thighLDir) * initialThighLRots[j];
        float scaleThigh = kneeDist / thighLengthLs[j];
        bShinLs[j].localPosition = initShinLLocals[j] * scaleThigh;
        Vector3 toAnkle = effectiveLeftAnkle - kneeL;
        float ankleDist = toAnkle.magnitude;
        float minShin = shinLengthLs[j] * minLegStretchRatio;
        float maxShin = shinLengthLs[j] * maxLegStretch;
        if (ankleDist > maxShin) { effectiveLeftAnkle = kneeL + toAnkle.normalized * maxShin; ankleDist = maxShin; }
        else if (ankleDist < minShin) { effectiveLeftAnkle = kneeL + toAnkle.normalized * minShin; ankleDist = minShin; }
        Vector3 shinLDir = (effectiveLeftAnkle - kneeL).normalized;
        bShinLs[j].rotation = Quaternion.FromToRotation(initShinLDirs[j], shinLDir) * initialShinLRots[j];
        float scaleShin = ankleDist / shinLengthLs[j];
        bFootLs[j].localPosition = initFootLLocals[j] * scaleShin;
        bFootLs[j].rotation = initialFootLRots[j];
        // Right leg
        Vector3 rightHipRoot = initRightHipPoss[j];
        Vector3 rightAnkleTarget = smoothedTargets[j][12];
        Vector3 rightKneeHint = smoothedTargets[j][11];
        Vector3 effectiveRightAnkle;
        Vector3 kneeR = SolveTwoBoneIK(rightHipRoot, rightAnkleTarget, rightKneeHint, thighLengthRs[j], shinLengthRs[j], legPoleStrength, out effectiveRightAnkle, legPreferredBendDirection);
        Vector3 toKneeR = kneeR - rightHipRoot;
        float kneeDistR = toKneeR.magnitude;
        float minThighR = thighLengthRs[j] * minLegStretchRatio;
        float maxThighR = thighLengthRs[j] * maxLegStretch;
        if (kneeDistR > maxThighR) { kneeR = rightHipRoot + toKneeR.normalized * maxThighR; kneeDistR = maxThighR; }
        else if (kneeDistR < minThighR) { kneeR = rightHipRoot + toKneeR.normalized * minThighR; kneeDistR = minThighR; }
        Vector3 thighRDir = (kneeR - rightHipRoot).normalized;
        bThighRs[j].rotation = Quaternion.FromToRotation(initThighRDirs[j], thighRDir) * initialThighRRots[j];
        float scaleThighR = kneeDistR / thighLengthRs[j];
        bShinRs[j].localPosition = initShinRLocals[j] * scaleThighR;
        Vector3 toAnkleR = effectiveRightAnkle - kneeR;
        float ankleDistR = toAnkleR.magnitude;
        float minShinR = shinLengthRs[j] * minLegStretchRatio;
        float maxShinR = shinLengthRs[j] * maxLegStretch;
        if (ankleDistR > maxShinR) { effectiveRightAnkle = kneeR + toAnkleR.normalized * maxShinR; ankleDistR = maxShinR; }
        else if (ankleDistR < minShinR) { effectiveRightAnkle = kneeR + toAnkleR.normalized * minShinR; ankleDistR = minShinR; }
        Vector3 shinRDir = (effectiveRightAnkle - kneeR).normalized;
        bShinRs[j].rotation = Quaternion.FromToRotation(initShinRDirs[j], shinRDir) * initialShinRRots[j];
        float scaleShinR = ankleDistR / shinLengthRs[j];
        bFootRs[j].localPosition = initFootRLocals[j] * scaleShinR;
        bFootRs[j].rotation = initialFootRRots[j];
    }

    private void ApplyArmIK(Vector3 shoulder, Vector3 elbowTarget, Vector3 wristTarget,
                            float upperLen, float foreLen,
                            Vector3 initUpperDir, Vector3 initForeDir,
                            Quaternion initUpperRot, Quaternion initForeRot,
                            Transform upper, Transform fore, Transform hand,
                            Quaternion relativeHandRot, Vector3 handOffset,
                            Vector3 initForearmLocal, Vector3 initHandLocal)
    {
        Vector3 toElbow = elbowTarget - shoulder;
        float elbowDist = toElbow.magnitude;
        if (elbowDist > upperLen * maxArmStretch) { elbowTarget = shoulder + toElbow.normalized * (upperLen * maxArmStretch); elbowDist = upperLen * maxArmStretch; }
        else if (elbowDist < upperLen * 0.3f) { elbowTarget = shoulder + toElbow.normalized * (upperLen * 0.3f); elbowDist = upperLen * 0.3f; }
        Vector3 upperDir = toElbow.normalized;
        upper.rotation = Quaternion.FromToRotation(initUpperDir, upperDir) * initUpperRot;
        float scaleUpper = elbowDist / upperLen;
        fore.localPosition = initForearmLocal * scaleUpper;

        Vector3 toWrist = wristTarget - elbowTarget;
        float wristDist = toWrist.magnitude;
        if (wristDist > foreLen * maxArmStretch) { wristTarget = elbowTarget + toWrist.normalized * (foreLen * maxArmStretch); wristDist = foreLen * maxArmStretch; }
        else if (wristDist < foreLen * 0.3f) { wristTarget = elbowTarget + toWrist.normalized * (foreLen * 0.3f); wristDist = foreLen * 0.3f; }
        Vector3 foreDir = toWrist.normalized;
        fore.rotation = Quaternion.FromToRotation(initForeDir, foreDir) * initForeRot;
        float scaleFore = wristDist / foreLen;
        hand.localPosition = initHandLocal * scaleFore;
        hand.rotation = fore.rotation * relativeHandRot * Quaternion.Euler(handOffset);
    }

    private Vector3 SolveTwoBoneIK(Vector3 root, Vector3 target, Vector3 pole, float len1, float len2, float poleStrength, out Vector3 effectiveTarget, Vector3 preferredBendDir = default)
    {
        effectiveTarget = target;
        Vector3 toTarget = effectiveTarget - root;
        float dist = toTarget.magnitude;
        if (dist > len1 + len2) { effectiveTarget = root + toTarget.normalized * (len1 + len2); dist = len1 + len2; toTarget = effectiveTarget - root; }
        float cosMin = Mathf.Cos(minKneeAngle * Mathf.Deg2Rad);
        float distSqMin = len1 * len1 + len2 * len2 - 2 * len1 * len2 * cosMin;
        float minDist = Mathf.Sqrt(Mathf.Max(distSqMin, 0f));
        if (dist < minDist) { effectiveTarget = root + toTarget.normalized * minDist; dist = minDist; toTarget = effectiveTarget - root; }
        float a = len1;
        float c = dist;
        float elbowDist = (a * a - len2 * len2 + c * c) / (2 * c);
        float hSq = a * a - elbowDist * elbowDist;
        float h = hSq > 0f ? Mathf.Sqrt(hSq) : 0f;
        h *= poleStrength;
        Vector3 rootToTargetN = toTarget.normalized;
        Vector3 rootToPole = pole - root;
        Vector3 bendNormal = Vector3.right;
        Vector3 rootToPoleProjected = rootToPole - Vector3.Dot(rootToPole, bendNormal) * bendNormal;
        Vector3 proj = rootToPoleProjected - Vector3.Dot(rootToPoleProjected, rootToTargetN) * rootToTargetN;
        Vector3 perp;
        if (proj.sqrMagnitude > 0.0001f) perp = proj.normalized;
        else perp = (preferredBendDir - Vector3.Dot(preferredBendDir, rootToTargetN) * rootToTargetN).normalized;
        float signDot = Vector3.Dot(perp, preferredBendDir.normalized);
        if (h < 0.05f || signDot < 0)
        {
            if (signDot < 0) { float ratio = len1 / (len1 + len2); return root + rootToTargetN * (dist * ratio); }
            else { float projLen = Vector3.Dot(rootToPole, rootToTargetN); projLen = Mathf.Clamp(projLen, 0f, dist); return root + rootToTargetN * projLen; }
        }
        Vector3 elbowBase = root + toTarget.normalized * elbowDist;
        Vector3 chosen = elbowBase + perp * h;
        return chosen;
    }

    private void SaveRecording()
    {
        if (recordedFrames.Count == 0) return;
        string path = System.IO.Path.Combine(Application.persistentDataPath, "WebcamRecording.json");
        var wrapper = new RecordingWrapper { frames = recordedFrames };
        string json = JsonUtility.ToJson(wrapper, true);
        System.IO.File.WriteAllText(path, json);
        Debug2.Log($"[RECORD] Saved {recordedFrames.Count} frames to {path}");
    }

    private void LoadRecording()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "WebcamRecording.json");
        if (!System.IO.File.Exists(path)) return;
        string json = System.IO.File.ReadAllText(path);
        var wrapper = JsonUtility.FromJson<RecordingWrapper>(json);
        if (wrapper?.frames != null)
        {
            recordedFrames = wrapper.frames;
            Debug2.Log($"[RECORD] Loaded {recordedFrames.Count} frames from disk");
        }
    }

    private void LogDebugPositions()
    {
        StringBuilder full = new StringBuilder("Debug Pose Positions over 5 seconds:\n");
        full.AppendLine($"matchTolerance: {matchTolerance}");
        full.AppendLine($"startDelaySeconds: {startDelaySeconds}");
        full.AppendLine($"requiredStartAccuracy: {requiredStartAccuracy}");
        full.AppendLine($"repetitionsCount: {repetitionsCount}");
        full.AppendLine($"maxAccuracyDropPerSecond: {maxAccuracyDropPerSecond}");
#if UNITY_EDITOR
        full.AppendLine($"accuracyOffset: {accuracyOffset.ToString()}");
        full.AppendLine($"instructionOffset: {instructionOffset.ToString()}");
#endif
        for (int k = 0; k < debugSnapshots.Count; k++)
        {
            full.AppendLine($"Snapshot {k}:\n" + debugSnapshots[k]);
        }
        Debug2.Log(full.ToString());
        debugSnapshots.Clear();
    }

    private string BuildDebugSnapshot()
    {
        StringBuilder sb = new StringBuilder();
        Vector3 center0 = (smoothedTargets[0][1] + smoothedTargets[0][4] + smoothedTargets[0][7] + smoothedTargets[0][10]) / 4f;
        Vector3 center1 = (smoothedTargets[1][1] + smoothedTargets[1][4] + smoothedTargets[1][7] + smoothedTargets[1][10]) / 4f;

        Vector3 shoulder_vec0 = smoothedTargets[0][4] - smoothedTargets[0][1];
        Vector3 shoulder_vec1 = smoothedTargets[1][4] - smoothedTargets[1][1];
        Quaternion alignment_rot = Quaternion.FromToRotation(shoulder_vec0.normalized, shoulder_vec1.normalized);
        float angle = Quaternion.Angle(alignment_rot, Quaternion.identity);
        sb.AppendLine($"Alignment Rotation Angle: {angle:F2} degrees");

        Vector3 mid_shoulder0 = (smoothedTargets[0][1] + smoothedTargets[0][4]) / 2f;
        Vector3 mid_hip0 = (smoothedTargets[0][7] + smoothedTargets[0][10]) / 2f;
        float torso_length0 = Vector3.Distance(mid_shoulder0, mid_hip0);

        Vector3 mid_shoulder1 = (smoothedTargets[1][1] + smoothedTargets[1][4]) / 2f;
        Vector3 mid_hip1 = (smoothedTargets[1][7] + smoothedTargets[1][10]) / 2f;
        float torso_length1 = Vector3.Distance(mid_shoulder1, mid_hip1);

        float scale_factor = (torso_length0 > 0f && torso_length1 > 0f) ? torso_length1 / torso_length0 : 1f;
        sb.AppendLine($"Scale Factor (teacher_torso / student_torso): {scale_factor:F4}");

        float acc = 0f;
        for (int i = 0; i < 13; i++)
        {
            Vector3 pos0_centered = smoothedTargets[0][i] - center0;
            Vector3 pos0_rotated = alignment_rot * pos0_centered;
            Vector3 pos0 = pos0_rotated * scale_factor;
            Vector3 pos1 = smoothedTargets[1][i] - center1;
            float dist = Vector3.Distance(pos0, pos1);
            float contrib = Mathf.Clamp01(1f - (dist / matchTolerance));
            sb.AppendLine($"Point {i}: Student (rotated & scaled) {pos0.ToString("F4")}, Teacher {pos1.ToString("F4")}, Dist {dist:F4}, Contrib {contrib:F4}");
            acc += contrib;
        }
        sb.AppendLine($"Total Accuracy: {(acc / 13f) * 100f:F1}%");
        return sb.ToString();
    }

    [System.Serializable]
    private class RecordingWrapper
    {
        public List<PoseFrame> frames = new List<PoseFrame>();
    }
}