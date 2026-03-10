BODY POSE TRACKING AND MATCHING SYSTEM

This repository contains a complete Unity application for real time body pose detection from webcam input. The system uses MediaPipe to track 13 body landmarks. It binds these points to two rigged 3D character models named student and teacher. The application supports live control of the student model and recorded playback on the teacher model with automatic accuracy scoring.

DESCRIPTION

The WebCam script handles MediaPipe annotation objects. It extracts positions for head left arm right arm left leg and right leg. It also applies a custom bulge distortion shader to the video feed for visual feedback on head body arms and legs. Parameters for each distortion part can be adjusted in the inspector.

The WebcamToModelBinder script receives these positions and drives the student and teacher models. It uses two bone inverse kinematics for arms and legs with stretch limits pole targets and knee angle constraints. Head rotation is calculated from shoulder and nose positions with smoothing and dead zones. The system includes recording of pose frames to JSON files playback mode and a matching phase that scores alignment over multiple repetitions.

Supporting classes provide extended logging delayed method calls and editor only visualization tools such as point icons and world space labels.

FEATURES

Real time extraction of 13 body landmarks from MediaPipe.  
Binding of landmarks to humanoid rigs with custom IK solvers for arms and legs.  
Custom bulge distortion shader applied to raw webcam image with individual controls for head body arms and legs.  
Pose recording at fixed interval saved as JSON to persistent data path.  
Playback mode with automatic delay removal for clean loops.  
Matching logic that rotates scales and compares student pose to teacher pose.  
Accuracy calculation based on per point distance within tolerance.  
Support for multiple repetitions with per repetition and overall average scores.  
Smoothing lag simulation and auto alignment for natural movement.  
Custom Debug2 logger with buffer pruning to 2048 entries and clipboard export.  
Editor tools for point visualization using shape icons and gizmo labels.

TECHNOLOGIES USED

Unity game engine.  
MediaPipe Unity package for pose detection.  
C Sharp for all game logic and IK calculations.  
ShaderLab for the bulge distortion effect.  
Unity UI and Text components for on screen debug information.  
JsonUtility for recording serialization.  
PlayerPrefs2 for optional quick save of recordings.

INSTALLATION

Clone the repository to a local folder.  
Open the folder in Unity Hub using a recent Unity version that supports the MediaPipe package.  
Import the official MediaPipe Unity Sample package from the Unity Asset Store or GitHub.  
Place the WebCam prefab or script on the main screen object that displays the webcam feed.  
Add the WebcamToModelBinder script to an empty game object.  
Assign the student and teacher models in the inspector. These models must contain the exact bone hierarchy B root B hips B spine B chest B neck B head and corresponding arm and leg bones.  
Assign the annotation root transforms and UI references as shown in the WebCam inspector.  
Set the bulge distortion shader and texture references if visual effects are required.

USAGE

Play the scene in the Unity editor or run a built executable.  
The student model follows live webcam input at all times.  
Press Q to begin recording pose data.  
Press W to stop recording and save the file to persistent data path.  
Press E to start playback and matching mode.  
In matching mode the teacher model replays the recording while the student model continues to follow the live webcam.  
The system automatically calculates accuracy every frame and displays it above the student model.  
After the required number of repetitions the application logs per repetition and overall average scores to the console.  
Press D to run a position logging coroutine for debugging.

All keyboard controls are active only in play mode. Debug text on screen shows current distortion states UI visibility and skeleton status. The custom logger records every important event with timestamps and can be copied to clipboard at any time.

This project shows practical experience with real time computer vision integration inverse kinematics shader programming state management and data persistence in Unity. The code is ready for extension such as adding new body parts or different matching metrics.

The repository contains only the necessary source files. No external assets are included except the required MediaPipe dependency. All references in scripts point to standard Unity and MediaPipe classes.