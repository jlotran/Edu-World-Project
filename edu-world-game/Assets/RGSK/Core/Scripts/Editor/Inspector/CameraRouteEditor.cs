using UnityEngine;
using UnityEditor;
using Cinemachine;

namespace RGSK.Editor
{
    [CustomEditor(typeof(CameraRoute))]
    public class CameraRouteEditor : UnityEditor.Editor
    {
        CameraRoute _target;
        string[] tabs = new string[] { "Look At", "Fixed", "Dolly" };
        static int tabIndex;

        void OnEnable()
        {
            _target = (CameraRoute)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Use the scene-view camera to position and then click on the 'create' button to place a camera", MessageType.Info);
            tabIndex = GUILayout.Toolbar(tabIndex, tabs, CustomEditorStyles.horizontalToolbarButton);
            GameObject template = null;

            if (GUILayout.Button("Create Route Camera Start", GUILayout.Height(25)))
            {
                switch (tabIndex)
                {
                    case 0:
                        {
                            template = RGSKEditorSettings.Instance.lookAtCameraTemplate;
                            break;
                        }
                    case 1:
                        {
                            template = RGSKEditorSettings.Instance.fixedCameraTemplate;
                            break;
                        }
                    case 2:
                        {
                            template = RGSKEditorSettings.Instance.dollyCameraTemplate;
                            break;
                        }
                }
                CreateRouteCamera("Route Camera Start", template);
            }

            if (GUILayout.Button("Create Route Camera End", GUILayout.Height(25)))
            {
                switch (tabIndex)
                {
                    case 0:
                        {
                            template = RGSKEditorSettings.Instance.lookAtCameraTemplate;
                            break;
                        }
                    case 1:
                        {
                            template = RGSKEditorSettings.Instance.fixedCameraTemplate;
                            break;
                        }
                    case 2:
                        {
                            template = RGSKEditorSettings.Instance.dollyCameraTemplate;
                            break;
                        }
                }
                CreateRouteCamera("Route Camera End", template);
            }

            if (GUI.changed)
            {
                _target.GetCameras();
            }

            EditorHelper.DrawLine();
            DrawDefaultInspector();
        }

        void CreateRouteCamera(string parentName, GameObject template)
        {
            if (template != null && SceneView.lastActiveSceneView.camera != null)
            {
                // Tạo GameObject parent nếu chưa có
                Transform parentTransform = _target.transform.Find(parentName);
                if (parentTransform == null)
                {
                    GameObject parentObject = new GameObject(parentName);
                    parentObject.transform.SetParent(_target.transform);
                    parentObject.transform.localPosition = Vector3.zero;
                    parentObject.transform.localRotation = Quaternion.identity;

                    var group = parentObject.AddComponent<RouteCameraGroup>();

                    Undo.RegisterCreatedObjectUndo(parentObject, "Create " + parentName + " Parent");
                    parentTransform = parentObject.transform;
                }

                // Tạo camera và gắn vào parent vừa tạo
                var cam = EditorHelper.CreatePrefab(template, parentTransform, Vector3.zero, Quaternion.identity, false, false);
                var pos = SceneView.lastActiveSceneView.camera.transform.position;
                var rot = SceneView.lastActiveSceneView.camera.transform.rotation;
                var rc = cam.GetComponentInChildren<RouteCamera>();
                var path = cam.GetComponentInChildren<CinemachineSmoothPath>();

                if (rc != null)
                {
                    rc.transform.SetPositionAndRotation(pos, rot);
                }

                if (path != null)
                {
                    path.transform.SetPositionAndRotation(pos, Quaternion.identity);
                }

                if (_target.route != null && _target.route.nodes.Count > 1)
                {
                    if (rc != null)
                    {
                        rc.routeDistance = _target.route.GetDistanceAtPosition(pos);
                    }
                }

                Undo.RegisterCreatedObjectUndo(cam, "created_cinematiccamera");
            }
        }
    }
}
