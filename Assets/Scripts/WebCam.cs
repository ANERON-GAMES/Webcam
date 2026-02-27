using UnityEngine;
using System.Collections;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;
using TNRD.Utilities;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class WebCam : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform annotatableScreen;
    [SerializeField] private AutoFit autoFit;
    [SerializeField] private GameObject[] allUI;
    [Header("Body Tracking Data")]
    [SerializeField] private Transform headPoint;
    [SerializeField] private Transform[] leftArm;
    [SerializeField] private Transform[] rightArm;
    [SerializeField] private Transform[] leftLeg;
    [SerializeField] private Transform[] rightLeg;
    private GameObject connectionListAnnotation;
    private PointAnnotation[] pointAnnotationScripts;
    [Header("Debugging")]
    [SerializeField] private GameObject debugObject;
    [SerializeField] private Text textMode;
    [Header("Z Position")]
    private GameObject annotationRoot;
    private Vector3 centerWorldPos = Vector3.zero;
    [SerializeField] private float zValue = 0f;
    [SerializeField] private float referenceZ = 2.4f;
    [SerializeField] private Text textPositionZ;
    [Header("Distortion Effect")]
    [SerializeField] private Shader bulgeDistortion;
    [Header("Head Distortion")]
    [SerializeField] private bool enableBigHead = true;
    [SerializeField] private Texture textureHead;
    [SerializeField] private Vector2 centerOffsetHead = Vector2.zero;
    [SerializeField][Range(0.01f, 1.0f)] private float radiusHead = 0.3f;
    [SerializeField] private Vector2 scaleHead = new Vector2(1, 1);
    [SerializeField][Range(-5f, 5f)] private float strengthHead = 0.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float softnessHead = 0.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float featherDistanceHead = 0.2f;
    [Header("Body Distortion")]
    [SerializeField] private bool enableBigBody = true;
    [SerializeField] private Texture textureBody;
    [SerializeField] private Vector2 centerOffsetBody = Vector2.zero;
    [SerializeField][Range(0.01f, 1.0f)] private float radiusBody = 0.3f;
    [SerializeField] private Vector2 scaleBody = new Vector2(1, 1);
    [SerializeField][Range(-5f, 5f)] private float strengthBody = 0.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float softnessBody = 0.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float featherDistanceBody = 0.2f;
    private Vector3 bodyWorldPos = Vector3.zero;
    [Header("Left Hand Distortion")]
    [SerializeField] private Texture textureLeftHand;
    [SerializeField] private Vector2 centerOffsetLeftHand = Vector2.zero;
    [SerializeField][Range(0.01f, 1.0f)] private float radiusLeftHand = 0.3f;
    [SerializeField] private Vector2 scaleLeftHand = new Vector2(1, 1);
    [SerializeField][Range(-5f, 5f)] private float strengthLeftHand = 0.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float softnessLeftHand = 0.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float featherDistanceLeftHand = 0.2f;
    [Header("Right Hand Distortion")]
    [SerializeField] private Texture textureRightHand;
    [SerializeField] private Vector2 centerOffsetRightHand = Vector2.zero;
    [SerializeField][Range(0.01f, 1.0f)] private float radiusRightHand = 0.3f;
    [SerializeField] private Vector2 scaleRightHand = new Vector2(1, 1);
    [SerializeField][Range(-5f, 5f)] private float strengthRightHand = 0.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float softnessRightHand = 0.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float featherDistanceRightHand = 0.2f;
    [Header("Left Arm Distortion")]
    [SerializeField] private bool enableBigLeftArm = true;
    [SerializeField] private Texture textureLeftArm;
    [SerializeField] private Vector2 centerOffsetLeftArm = Vector2.zero;
    [SerializeField][Range(0.01f, 1.0f)] private float radiusLeftArm = 0.3f;
    [SerializeField] private Vector2 scaleLeftArm = new Vector2(1, 1);
    [SerializeField][Range(-5f, 5f)] private float strengthLeftArm = 0.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float softnessLeftArm = 0.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float featherDistanceLeftArm = 0.2f;
    [SerializeField] private Vector2 paddingLeftArmSegment1 = Vector2.zero;
    [SerializeField] private Vector2 paddingLeftArmSegment2 = Vector2.zero;
    [Header("Right Arm Distortion")]
    [SerializeField] private bool enableBigRightArm = true;
    [SerializeField] private Texture textureRightArm;
    [SerializeField] private Vector2 centerOffsetRightArm = Vector2.zero;
    [SerializeField][Range(0.01f, 1.0f)] private float radiusRightArm = 0.3f;
    [SerializeField] private Vector2 scaleRightArm = new Vector2(1, 1);
    [SerializeField][Range(-5f, 5f)] private float strengthRightArm = 0.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float softnessRightArm = 0.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float featherDistanceRightArm = 0.2f;
    [SerializeField] private Vector2 paddingRightArmSegment1 = Vector2.zero;
    [SerializeField] private Vector2 paddingRightArmSegment2 = Vector2.zero;
    [Header("Left Leg Distortion")]
    [SerializeField] private bool enableBigLeftLeg = true;
    [SerializeField] private Texture textureLeftLeg;
    [SerializeField] private Vector2 centerOffsetLeftLeg = Vector2.zero;
    [SerializeField][Range(0.01f, 1.0f)] private float radiusLeftLeg = 0.3f;
    [SerializeField] private Vector2 scaleLeftLeg = new Vector2(1, 1);
    [SerializeField][Range(-5f, 5f)] private float strengthLeftLeg = 0.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float softnessLeftLeg = 0.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float featherDistanceLeftLeg = 0.2f;
    [SerializeField] private Vector2 paddingLeftLegSegment1 = Vector2.zero;
    [SerializeField] private Vector2 paddingLeftLegSegment2 = Vector2.zero;
    [Header("Right Leg Distortion")]
    [SerializeField] private bool enableBigRightLeg = true;
    [SerializeField] private Texture textureRightLeg;
    [SerializeField] private Vector2 centerOffsetRightLeg = Vector2.zero;
    [SerializeField][Range(0.01f, 1.0f)] private float radiusRightLeg = 0.3f;
    [SerializeField] private Vector2 scaleRightLeg = new Vector2(1, 1);
    [SerializeField][Range(-5f, 5f)] private float strengthRightLeg = 0.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float softnessRightLeg = 0.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float featherDistanceRightLeg = 0.2f;
    [SerializeField] private Vector2 paddingRightLegSegment1 = Vector2.zero;
    [SerializeField] private Vector2 paddingRightLegSegment2 = Vector2.zero;
    private RawImage rawImage;
    private Material bulgeMaterial;
    public GameObject AnnotationRoot => annotationRoot;
    public float ZValue => zValue;
    public Vector3 GetBodyCenterWorld() => bodyWorldPos;
    public float ReferenceZ => referenceZ;
    public RectTransform AnnotatableScreen => annotatableScreen;
    private void Start()
    {
        if (autoFit != null) StartCoroutine(InitialScreenSetup());
        Invoke(nameof(InitializeBodyPoints), 3.0f);
        Invoke(nameof(ToggleConnectionAnnotation), 4f);
        Invoke(nameof(InitializeBulgeEffect), 5f);
        Invoke(nameof(ToggleUI), 5f);
        Invoke(nameof(SetMaterialParameters), 6f);
        Invoke(nameof(DisableBulgeEffect), 7f);
        DelayedMethodInvoker.Instance.StartMethodDelayed(() => textPositionZ.gameObject.SetActive(!textPositionZ.gameObject.activeSelf), 10f);
    }
    private void OnValidate()
    {
        if (Application.isPlaying && bulgeMaterial != null)
        {
            SetMaterialParameters();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) ToggleBulgeEffect();
        if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleUI();
        if (Input.GetKeyDown(KeyCode.Alpha2)) ToggleConnectionAnnotation();
        if (Input.GetKeyDown(KeyCode.Alpha3)) textPositionZ.gameObject.SetActive(!textPositionZ.gameObject.activeSelf);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ToggleHead();
        if (Input.GetKeyDown(KeyCode.Alpha5)) ToggleLeftArm();
        if (Input.GetKeyDown(KeyCode.Alpha6)) ToggleRightArm();
        if (Input.GetKeyDown(KeyCode.Alpha7)) ToggleLeftLeg();
        if (Input.GetKeyDown(KeyCode.Alpha8)) ToggleRightLeg();
        if (Input.GetKeyDown(KeyCode.Alpha9)) ToggleBody();
        if (bulgeMaterial != null && rawImage != null && annotatableScreen != null)
        {
            float depthFactor = referenceZ / Mathf.Max(zValue, 0.01f);
            // Head
            if (headPoint != null)
            {
                Vector2 normalizedUV = GetNormalizedUV(headPoint);
                bulgeMaterial.SetVector("_Center_Head", normalizedUV);
                bulgeMaterial.SetFloat("_Angle_Head", 0f);
                bulgeMaterial.SetFloat("_DepthFactor_Head", depthFactor);
            }
            // Left Hand
            if (leftArm.Length > 2 && leftArm[2] != null)
            {
                Vector2 normalizedUV = GetNormalizedUV(leftArm[2]);
                bulgeMaterial.SetVector("_Center_LeftHand", normalizedUV);
                bulgeMaterial.SetFloat("_Angle_LeftHand", 0f);
                bulgeMaterial.SetFloat("_DepthFactor_LeftHand", depthFactor);
            }
            // Right Hand
            if (rightArm.Length > 2 && rightArm[2] != null)
            {
                Vector2 normalizedUV = GetNormalizedUV(rightArm[2]);
                bulgeMaterial.SetVector("_Center_RightHand", normalizedUV);
                bulgeMaterial.SetFloat("_Angle_RightHand", 0f);
                bulgeMaterial.SetFloat("_DepthFactor_RightHand", depthFactor);
            }
            // Limbs (all use exactly 3 points / 2 segments)
            ProcessLimb(leftArm, radiusLeftArm, scaleLeftArm, paddingLeftArmSegment1, paddingLeftArmSegment2,
                "_LeftArm1", "_LeftArm2", depthFactor);
            ProcessLimb(rightArm, radiusRightArm, scaleRightArm, paddingRightArmSegment1, paddingRightArmSegment2,
                "_RightArm1", "_RightArm2", depthFactor);
            ProcessLimb(leftLeg, radiusLeftLeg, scaleLeftLeg, paddingLeftLegSegment1, paddingLeftLegSegment2,
                "_LeftLeg1", "_LeftLeg2", depthFactor);
            ProcessLimb(rightLeg, radiusRightLeg, scaleRightLeg, paddingRightLegSegment1, paddingRightLegSegment2,
                "_RightLeg1", "_RightLeg2", depthFactor);
            rawImage.SetMaterialDirty();
        }
        if (annotationRoot != null && headPoint != null && leftArm.Length > 0 && leftArm[0] != null && rightArm.Length > 0 && rightArm[0] != null)
        {
            Vector3 headPos = headPoint.localPosition;
            Vector3 leftShoulder = leftArm[0].localPosition;
            Vector3 rightShoulder = rightArm[0].localPosition;
            Vector3 centerLocal = (headPos + leftShoulder + rightShoulder) / 3f;
            centerWorldPos = annotationRoot.transform.TransformPoint(centerLocal);
            float dist = Vector2.Distance(new Vector2(leftShoulder.x, leftShoulder.y), new Vector2(rightShoulder.x, rightShoulder.y));
            float width = annotatableScreen.rect.width;
            zValue = width / dist;
        }
        if (leftArm.Length > 0 && leftArm[0] != null && rightArm.Length > 0 && rightArm[0] != null && leftLeg.Length > 0 && leftLeg[0] != null && rightLeg.Length > 0 && rightLeg[0] != null)
        {
            Vector3 leftShoulder = leftArm[0].localPosition;
            Vector3 rightShoulder = rightArm[0].localPosition;
            Vector3 leftHip = leftLeg[0].localPosition;
            Vector3 rightHip = rightLeg[0].localPosition;
            Vector3 bodyLocal = (leftShoulder + rightShoulder + leftHip + rightHip) / 4f;
            bodyWorldPos = annotationRoot.transform.TransformPoint(bodyLocal);
            if (bulgeMaterial != null)
            {
                float depthFactor = referenceZ / Mathf.Max(zValue, 0.01f);
                Vector2 normalizedUV = GetNormalizedUVFromLocal(bodyLocal);
                bulgeMaterial.SetVector("_Center_Body", normalizedUV);
                bulgeMaterial.SetFloat("_Angle_Body", 0f);
                bulgeMaterial.SetFloat("_DepthFactor_Body", depthFactor);
            }
        }
        if (textPositionZ != null && textPositionZ.gameObject.activeSelf == true)
        {
            textPositionZ.text = $"Z: {zValue:F1}";
        }
        UpdateDebugText();
    }
    private void UpdateDebugText()
    {
        if (textMode == null) return;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("Enabled Distortions:");
        sb.AppendLine($"Head: {(enableBigHead ? "On" : "Off")}");
        sb.AppendLine($"Left Arm (incl. Hand): {(enableBigLeftArm ? "On" : "Off")}");
        sb.AppendLine($"Right Arm (incl. Hand): {(enableBigRightArm ? "On" : "Off")}");
        sb.AppendLine($"Left Leg: {(enableBigLeftLeg ? "On" : "Off")}");
        sb.AppendLine($"Right Leg: {(enableBigRightLeg ? "On" : "Off")}");
        sb.AppendLine($"Body: {(enableBigBody ? "On" : "Off")}");
        sb.AppendLine();
        sb.AppendLine("Key Bindings:");
        sb.AppendLine("0: Toggle Bulge Effect (Overall Distortion)");
        sb.AppendLine("1: Toggle UI");
        sb.AppendLine("2: Toggle Skeleton (Connections & Points)");
        sb.AppendLine("3: Toggle Z Value Display");
        sb.AppendLine("4: Toggle Head Distortion");
        sb.AppendLine("5: Toggle Left Arm Distortion");
        sb.AppendLine("6: Toggle Right Arm Distortion");
        sb.AppendLine("7: Toggle Left Leg Distortion");
        sb.AppendLine("8: Toggle Right Leg Distortion");
        sb.AppendLine("9: Toggle Body Distortion");
        sb.AppendLine();
        sb.AppendLine("Other Info:");
        string bulgeStatus = (rawImage == null || bulgeMaterial == null) ? "null" : (rawImage.material == bulgeMaterial ? "On" : "Off");
        sb.AppendLine($"Bulge Effect: {bulgeStatus}");
        sb.AppendLine($"UI Visible: {(allUI.Length > 0 && allUI[0] != null ? (allUI[0].activeSelf ? "Yes" : "No") : "N/A")}");
        sb.AppendLine($"Skeleton Visible: {(connectionListAnnotation != null ? (connectionListAnnotation.activeSelf ? "Yes" : "No") : "N/A")}");
        string zDisplayStatus = (textPositionZ == null) ? "null" : (textPositionZ.gameObject.activeSelf ? "On" : "Off");
        sb.AppendLine($"Z Display: {zDisplayStatus}");
        textMode.text = sb.ToString();
    }
    private void ProcessLimb(Transform[] points, float radius, Vector2 scale, Vector2 padding1, Vector2 padding2, string suffix1, string suffix2, float depthFactor)
    {
        if (points.Length < 3 || points[0] == null || points[1] == null || points[2] == null) return;
        Vector2 p0 = GetNormalizedUV(points[0]);
        Vector2 p1 = GetNormalizedUV(points[1]);
        Vector2 p2 = GetNormalizedUV(points[2]);
        ApplySegment(p0, p1, padding1, radius, scale, suffix1, depthFactor);
        ApplySegment(p1, p2, padding2, radius, scale, suffix2, depthFactor);
    }
    private void ApplySegment(Vector2 start, Vector2 end, Vector2 padding, float radius, Vector2 scale, string suffix, float depthFactor)
    {
        float len = Vector2.Distance(start, end);
        if (len <= 0.001f) return;
        float startFrac = Mathf.Clamp(padding.x, 0f, 0.49f);
        float endFrac = Mathf.Clamp(padding.y, 0f, 0.49f);
        Vector2 dir = (end - start).normalized;
        Vector2 newStart = start + dir * (len * startFrac);
        Vector2 newEnd = end - dir * (len * endFrac);
        float newLen = Vector2.Distance(newStart, newEnd);
        if (newLen <= 0.001f) return;
        Vector2 center = (newStart + newEnd) / 2f;
        float angle = Mathf.Atan2(newEnd.y - newStart.y, newEnd.x - newStart.x);
        float scaleY;
        if (suffix.Contains("Arm"))
        {
            angle = 0f;
            scaleY = scale.y;
        }
        else
        {
            scaleY = (newLen / 2f) / (radius * depthFactor) * scale.y;
        }
        bulgeMaterial.SetVector("_Center" + suffix, center);
        bulgeMaterial.SetFloat("_Angle" + suffix, angle);
        bulgeMaterial.SetVector("_Scale" + suffix, new Vector4(scale.x, scaleY, 0, 0));
    }
    public void InitializeBodyPoints()
    {
        annotationRoot = GameObject.Find("Point List Annotation");
        if (annotationRoot == null)
        {
            Debug2.LogWarning("[WebCam] 'Point List Annotation' not found on the scene. Try calling the method later.");
            return;
        }
        Transform[] allPoints = annotationRoot.GetComponentsInChildren<Transform>();
        headPoint = GetPoint(allPoints, 1);
        leftArm = new Transform[] { GetPoint(allPoints, 13), GetPoint(allPoints, 15), GetPoint(allPoints, 17) };
        rightArm = new Transform[] { GetPoint(allPoints, 12), GetPoint(allPoints, 14), GetPoint(allPoints, 16) };
        leftLeg = new Transform[] { GetPoint(allPoints, 25), GetPoint(allPoints, 27), GetPoint(allPoints, 29) }; // hip, knee, ankle
        rightLeg = new Transform[] { GetPoint(allPoints, 24), GetPoint(allPoints, 26), GetPoint(allPoints, 28) }; // hip, knee, ankle
#if UNITY_EDITOR
        if (headPoint != null)
        {
            IconManager.SetIcon(headPoint.gameObject, ShapeIcon.CircleGray);
        }
        foreach (var point in leftArm)
        {
            if (point != null)
            {
                IconManager.SetIcon(point.gameObject, ShapeIcon.CircleRed);
            }
        }
        foreach (var point in rightArm)
        {
            if (point != null)
            {
                IconManager.SetIcon(point.gameObject, ShapeIcon.CircleBlue);
            }
        }
        foreach (var point in leftLeg)
        {
            if (point != null)
            {
                IconManager.SetIcon(point.gameObject, ShapeIcon.CircleYellow);
            }
        }
        foreach (var point in rightLeg)
        {
            if (point != null)
            {
                IconManager.SetIcon(point.gameObject, ShapeIcon.CircleGreen);
            }
        }
#endif
        Debug2.Log("<color=green>[WebCam]</color> All body points successfully bound.");
        Debug2.Log("<color=green>[WebCam]</color> Z gizmo setup complete.");
    }
    public void InitializeBulgeEffect()
    {
        // Annotatable Screen
        if (annotatableScreen == null)
        {
            GameObject annotatableObj = GameObject.Find("Annotatable Screen");
            if (annotatableObj != null)
            {
                annotatableScreen = annotatableObj.GetComponent<RectTransform>();
            }
            else
            {
                Debug2.LogWarning("[WebCam] 'Annotatable Screen' not found on the scene.");
                return;
            }
        }
        // RawImage
        rawImage = annotatableScreen.GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug2.LogWarning("[WebCam] RawImage component not found on Annotatable Screen.");
            return;
        }
        //
        if (bulgeDistortion != null)
        {
            bulgeMaterial = new Material(bulgeDistortion);
            rawImage.material = bulgeMaterial;
            Debug2.Log("<color=green>[WebCam]</color> Bulge distortion material applied.");
        }
        else
        {
            Debug2.LogWarning("[WebCam] BulgeDistortion shader not assigned.");
        }
    }
    public void DisableBulgeEffect()
    {
        if (rawImage != null && bulgeMaterial != null)
        {
            rawImage.material = null;
        }
    }
    public void ToggleHead()
    {
        enableBigHead = !enableBigHead;
        SetMaterialParameters();
    }
    public void ToggleBulgeEffect()
    {
        if (rawImage == null || bulgeMaterial == null) return;
        if (rawImage.material == bulgeMaterial)
        {
            rawImage.material = null;
        }
        else
        {
            rawImage.material = bulgeMaterial;
            SetMaterialParameters(); // Refresh parameters if needed
        }
    }
    public void ToggleLeftArm()
    {
        enableBigLeftArm = !enableBigLeftArm;
        SetMaterialParameters();
    }
    public void ToggleRightArm()
    {
        enableBigRightArm = !enableBigRightArm;
        SetMaterialParameters();
    }
    public void ToggleLeftLeg()
    {
        enableBigLeftLeg = !enableBigLeftLeg;
        SetMaterialParameters();
    }
    public void ToggleRightLeg()
    {
        enableBigRightLeg = !enableBigRightLeg;
        SetMaterialParameters();
    }
    public void ToggleBody()
    {
        enableBigBody = !enableBigBody;
        SetMaterialParameters();
    }
    public void ToggleConnectionAnnotation()
    {
        if (connectionListAnnotation == null)
        {
            connectionListAnnotation = GameObject.Find("Connection List Annotation");
            if (connectionListAnnotation == null)
            {
                Debug2.LogWarning("[WebCam] 'Connection List Annotation' not found on the scene.");
                return;
            }
        }
        bool newState = !connectionListAnnotation.activeSelf;
        connectionListAnnotation.SetActive(newState);
        if (pointAnnotationScripts == null || pointAnnotationScripts.Length == 0)
        {
            GameObject pointListRoot = GameObject.Find("Point List Annotation");
            if (pointListRoot != null)
            {
                pointAnnotationScripts = pointListRoot.GetComponentsInChildren<PointAnnotation>();
            }
            else
            {
                Debug2.LogWarning("[WebCam] 'Point List Annotation' not found on the scene.");
                return;
            }
        }
        foreach (var script in pointAnnotationScripts)
        {
            if (script != null)
            {
                script.enabled = newState;
            }
        }
    }
    private Transform GetPoint(Transform[] points, int index)
    {
        if (index >= 0 && index < points.Length)
        {
            return points[index];
        }
        return null;
    }
    private Vector2 GetNormalizedUV(Transform point)
    {
        if (point == null) return Vector2.zero;
        float width = annotatableScreen.rect.width;
        float height = annotatableScreen.rect.height;
        Vector2 localPos = point.localPosition;
        float normX = (localPos.x + width / 2f) / width;
        float normY = (localPos.y + height / 2f) / height;
        return new Vector2(1f - normX, normY);
    }
    private Vector2 GetNormalizedUVFromLocal(Vector3 localPos)
    {
        float width = annotatableScreen.rect.width;
        float height = annotatableScreen.rect.height;
        float normX = (localPos.x + width / 2f) / width;
        float normY = (localPos.y + height / 2f) / height;
        return new Vector2(1f - normX, normY);
    }
    private IEnumerator InitialScreenSetup()
    {
        yield return new WaitForSeconds(2.0f);
        if (autoFit != null) autoFit.enabled = false;
        ScaleToFitHeight();
    }
    public void ScaleToFitHeight()
    {
        if (annotatableScreen == null) return;
        RectTransform parentRect = annotatableScreen.parent as RectTransform;
        float aspect = 1.777f;
        var imageSource = ImageSourceProvider.ImageSource;
        if (imageSource != null && imageSource.GetCurrentTexture() != null)
            aspect = (float)imageSource.GetCurrentTexture().width / imageSource.GetCurrentTexture().height;
        float parentHeight = parentRect.rect.height;
        float targetWidth = parentHeight * aspect;
        annotatableScreen.anchorMin = annotatableScreen.anchorMax = annotatableScreen.pivot = new Vector2(0.5f, 0.5f);
        annotatableScreen.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentHeight);
        annotatableScreen.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
        annotatableScreen.anchoredPosition = Vector2.zero;
    }
    public void ToggleUI()
    {
        if (allUI == null || allUI.Length == 0) return;
        bool newState = !allUI[0].activeSelf;
        debugObject.SetActive(newState);
        foreach (var ui in allUI) if (ui != null) ui.SetActive(newState);
    }
    private void SetMaterialParameters()
    {
        if (bulgeMaterial == null) return;
        // Head
        bulgeMaterial.SetInt("_Enable_Head", enableBigHead ? 1 : 0);
        bulgeMaterial.SetTexture("_DistortionTex_Head", textureHead);
        bulgeMaterial.SetVector("_CenterOffset_Head", centerOffsetHead);
        bulgeMaterial.SetFloat("_Radius_Head", radiusHead);
        bulgeMaterial.SetVector("_Scale_Head", scaleHead);
        bulgeMaterial.SetFloat("_Strength_Head", strengthHead);
        bulgeMaterial.SetFloat("_Softness_Head", softnessHead);
        bulgeMaterial.SetFloat("_FeatherDistance_Head", featherDistanceHead);
        // Body
        bulgeMaterial.SetInt("_Enable_Body", enableBigBody ? 1 : 0);
        bulgeMaterial.SetTexture("_DistortionTex_Body", textureBody);
        bulgeMaterial.SetVector("_CenterOffset_Body", centerOffsetBody);
        bulgeMaterial.SetFloat("_Radius_Body", radiusBody);
        bulgeMaterial.SetVector("_Scale_Body", scaleBody);
        bulgeMaterial.SetFloat("_Strength_Body", strengthBody);
        bulgeMaterial.SetFloat("_Softness_Body", softnessBody);
        bulgeMaterial.SetFloat("_FeatherDistance_Body", featherDistanceBody);
        // Left Hand
        bulgeMaterial.SetTexture("_DistortionTex_LeftHand", textureLeftHand);
        bulgeMaterial.SetVector("_CenterOffset_LeftHand", centerOffsetLeftHand);
        bulgeMaterial.SetFloat("_Radius_LeftHand", radiusLeftHand);
        bulgeMaterial.SetVector("_Scale_LeftHand", scaleLeftHand);
        bulgeMaterial.SetFloat("_Strength_LeftHand", strengthLeftHand);
        bulgeMaterial.SetFloat("_Softness_LeftHand", softnessLeftHand);
        bulgeMaterial.SetFloat("_FeatherDistance_LeftHand", featherDistanceLeftHand);
        // Right Hand
        bulgeMaterial.SetTexture("_DistortionTex_RightHand", textureRightHand);
        bulgeMaterial.SetVector("_CenterOffset_RightHand", centerOffsetRightHand);
        bulgeMaterial.SetFloat("_Radius_RightHand", radiusRightHand);
        bulgeMaterial.SetVector("_Scale_RightHand", scaleRightHand);
        bulgeMaterial.SetFloat("_Strength_RightHand", strengthRightHand);
        bulgeMaterial.SetFloat("_Softness_RightHand", softnessRightHand);
        bulgeMaterial.SetFloat("_FeatherDistance_RightHand", featherDistanceRightHand);
        // Left Arm
        bulgeMaterial.SetInt("_Enable_LeftArm", enableBigLeftArm ? 1 : 0);
        bulgeMaterial.SetTexture("_DistortionTex_LeftArm", textureLeftArm);
        bulgeMaterial.SetVector("_CenterOffset_LeftArm", centerOffsetLeftArm);
        bulgeMaterial.SetFloat("_Radius_LeftArm", radiusLeftArm);
        bulgeMaterial.SetFloat("_Strength_LeftArm", strengthLeftArm);
        bulgeMaterial.SetFloat("_Softness_LeftArm", softnessLeftArm);
        bulgeMaterial.SetFloat("_FeatherDistance_LeftArm", featherDistanceLeftArm);
        // Right Arm
        bulgeMaterial.SetInt("_Enable_RightArm", enableBigRightArm ? 1 : 0);
        bulgeMaterial.SetTexture("_DistortionTex_RightArm", textureRightArm);
        bulgeMaterial.SetVector("_CenterOffset_RightArm", centerOffsetRightArm);
        bulgeMaterial.SetFloat("_Radius_RightArm", radiusRightArm);
        bulgeMaterial.SetFloat("_Strength_RightArm", strengthRightArm);
        bulgeMaterial.SetFloat("_Softness_RightArm", softnessRightArm);
        bulgeMaterial.SetFloat("_FeatherDistance_RightArm", featherDistanceRightArm);
        // Left Leg
        bulgeMaterial.SetInt("_Enable_LeftLeg", enableBigLeftLeg ? 1 : 0);
        bulgeMaterial.SetTexture("_DistortionTex_LeftLeg", textureLeftLeg);
        bulgeMaterial.SetVector("_CenterOffset_LeftLeg", centerOffsetLeftLeg);
        bulgeMaterial.SetFloat("_Radius_LeftLeg", radiusLeftLeg);
        bulgeMaterial.SetFloat("_Strength_LeftLeg", strengthLeftLeg);
        bulgeMaterial.SetFloat("_Softness_LeftLeg", softnessLeftLeg);
        bulgeMaterial.SetFloat("_FeatherDistance_LeftLeg", featherDistanceLeftLeg);
        // Right Leg
        bulgeMaterial.SetInt("_Enable_RightLeg", enableBigRightLeg ? 1 : 0);
        bulgeMaterial.SetTexture("_DistortionTex_RightLeg", textureRightLeg);
        bulgeMaterial.SetVector("_CenterOffset_RightLeg", centerOffsetRightLeg);
        bulgeMaterial.SetFloat("_Radius_RightLeg", radiusRightLeg);
        bulgeMaterial.SetFloat("_Strength_RightLeg", strengthRightLeg);
        bulgeMaterial.SetFloat("_Softness_RightLeg", softnessRightLeg);
        bulgeMaterial.SetFloat("_FeatherDistance_RightLeg", featherDistanceRightLeg);
    }
    public Vector3[] GetAllPointPositions()
    {
        if (headPoint == null || leftArm == null || leftArm.Length < 3 || rightArm == null || rightArm.Length < 3 ||
            leftLeg == null || leftLeg.Length < 3 || rightLeg == null || rightLeg.Length < 3)
        {
            return null;
        }
        Vector3[] positions = new Vector3[13];
        positions[0] = headPoint.localPosition;
        positions[1] = leftArm[0].localPosition;
        positions[2] = leftArm[1].localPosition;
        positions[3] = leftArm[2].localPosition;
        positions[4] = rightArm[0].localPosition;
        positions[5] = rightArm[1].localPosition;
        positions[6] = rightArm[2].localPosition;
        positions[7] = leftLeg[0].localPosition;
        positions[8] = leftLeg[1].localPosition;
        positions[9] = leftLeg[2].localPosition;
        positions[10] = rightLeg[0].localPosition;
        positions[11] = rightLeg[1].localPosition;
        positions[12] = rightLeg[2].localPosition;
        return positions;
    }
    public void SetAnnotationsActive(bool active)
    {
        if (connectionListAnnotation == null)
        {
            connectionListAnnotation = GameObject.Find("Connection List Annotation");
        }
        if (connectionListAnnotation != null) connectionListAnnotation.SetActive(active);
        if (pointAnnotationScripts == null || pointAnnotationScripts.Length == 0)
        {
            GameObject pointListRoot = GameObject.Find("Point List Annotation");
            if (pointListRoot != null)
            {
                pointAnnotationScripts = pointListRoot.GetComponentsInChildren<PointAnnotation>();
            }
        }
        foreach (var script in pointAnnotationScripts)
        {
            if (script != null) script.enabled = active;
        }
    }
    public List<Transform> GetAllPointTransforms()
    {
        List<Transform> points = new List<Transform>();
        if (headPoint != null) points.Add(headPoint);
        if (leftArm != null && leftArm.Length == 3)
        {
            points.Add(leftArm[0]);
            points.Add(leftArm[1]);
            points.Add(leftArm[2]);
        }
        if (rightArm != null && rightArm.Length == 3)
        {
            points.Add(rightArm[0]);
            points.Add(rightArm[1]);
            points.Add(rightArm[2]);
        }
        if (leftLeg != null && leftLeg.Length == 3)
        {
            points.Add(leftLeg[0]);
            points.Add(leftLeg[1]);
            points.Add(leftLeg[2]);
        }
        if (rightLeg != null && rightLeg.Length == 3)
        {
            points.Add(rightLeg[0]);
            points.Add(rightLeg[1]);
            points.Add(rightLeg[2]);
        }
        if (points.Count == 13) return points;
        return null;
    }
    public void SetPointUpdates(bool enabled)
    {
        if (pointAnnotationScripts == null || pointAnnotationScripts.Length == 0)
        {
            GameObject pointListRoot = GameObject.Find("Point List Annotation");
            if (pointListRoot != null)
            {
                pointAnnotationScripts = pointListRoot.GetComponentsInChildren<PointAnnotation>();
            }
        }
        foreach (var script in pointAnnotationScripts)
        {
            if (script != null)
            {
                script.enabled = enabled;
            }
        }
        if (connectionListAnnotation == null)
        {
            connectionListAnnotation = GameObject.Find("Connection List Annotation");
        }
        if (connectionListAnnotation != null)
        {
            connectionListAnnotation.SetActive(enabled);
        }
    }
    [ContextMenu("Log Data")]
    public void LogAllValues()
    {
        PlayerPrefs2.LogAllValues();
    }
    [ContextMenu("DeleteAll")]
    public void DeleteAllValues()
    {
        PlayerPrefs2.DeleteAll();
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (centerWorldPos != Vector3.zero)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            Handles.Label(centerWorldPos + Vector3.up * 20f, $"Z: {zValue:F1}", style);
        }
        if (bodyWorldPos != Vector3.zero)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.cyan;
            Handles.Label(bodyWorldPos + Vector3.up * 20f, "body", style);
        }
    }
#endif
}