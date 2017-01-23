using UnityEngine;
using UnityEditor;
using System.Collections;

/// Scene Editor extension overlays the firing arc of projectile towers

//[CustomEditor(typeof(ProjectileTower))]
//public class ProjectileTowerEditor : Editor {
//
//    private void OnSceneGUI() {
//
//        ProjectileTower tower = (ProjectileTower)target;
//
//        Handles.color = new Color(1f, 1f, 1f, 0.1f);
//        Handles.DrawSolidArc(tower.transform.position, tower.transform.up, tower.transform.right, 360, tower.projectileTowerProperties.firingRange);
//
//        Handles.color = new Color(0.6f, 0.8f, 1f, 0.2f);
//        float range = Mathf.Abs(tower.targetingProperties.targetField.minimumRotation) + Mathf.Abs(tower.targetingProperties.targetField.maximumRotation);
//        Vector3 start = Quaternion.Euler(0, tower.targetingProperties.targetField.minimumRotation, 0) * tower.transform.forward;
//        Handles.DrawSolidArc(tower.transform.position, tower.transform.up, start, range, tower.projectileTowerProperties.firingRange);
//
//        if (tower.targetingProperties.fieldOfView) {
//            Handles.color = new Color(0.6f, 0.8f, 1f, 0.2f);
//            Vector3 start = Quaternion.Euler(0, -tower.targetingProperties.fieldOfViewAngle / 2, 0) * tower.turret.transform.forward;//new Vector3(0, tower.targetingProperties.fieldOfViewAngle / 2 + tower.turret.transform.root.eulerAngles.y, 0);//Quaternion.Euler(0, tower.transform.rotation.eulerAngles.y - tower.targetingProperties.fieldOfViewAngle / 2, 0);
//            Handles.DrawSolidArc(tower.transform.position, tower.transform.up, start, tower.targetingProperties.fieldOfViewAngle, tower.projectileTowerProperties.firingRange);
//        }
//
//        if(tower.target) {
//            Handles.color = Color.white;
//            Handles.DrawLine(tower.target.transform.position, tower.projectileTowerProperties.firingPoint.transform.position);
//        }
//    }
//
//}