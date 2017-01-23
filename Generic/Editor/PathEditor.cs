using UnityEngine;
using UnityEditor;
using System.Collections;

// GUI and Editor Extensions for Bezier paths

[CustomEditor(typeof(Path))]
public class PathEditor : Editor {

    private Path _curve;
    private Transform _transform;
    private Quaternion _rotation;

    private int _steps = 20;

    private int _selected = -1;

    private const float _handleSize = 0.1f;
    private const float _pickSize = 0.15f;

    private void buildLinks() {
        for (int idx = 1; idx < _curve.segments.Count; idx++) {
            Segment previous = _curve.segments[idx - 1];
            Segment next = _curve.segments[idx];
            previous.next = next;
            next.previous = previous;
        }
    }

    public Vector3 normalVector(Vector3 a, Vector3 b, Vector3 c) {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        return Vector3.Cross(ab, ac).normalized;
    }


    private void OnSceneGUI() {
        float stepSize = 1f / _steps;

        _curve = (Path)target;
        _transform = _curve.transform;
        _rotation = Tools.pivotRotation == PivotRotation.Local ? _transform.rotation : Quaternion.identity;

        if (_curve.segments.Count > 1 && _curve.segments[_curve.segments.Count - 1].next == null) {
            buildLinks();
        }

        for (int index = 0; index < _curve.Length; index++) {
            ShowPoint(index);
        }

        for (int index = 0; index < _curve.segments.Count; index++) {
            Segment segment = _curve.segments[index];

            Vector3 start = _transform.TransformPoint(segment.start);
            Vector3 control = _transform.TransformPoint(segment.control);
            Vector3 end = _transform.TransformPoint(segment.end);

            Vector3 normal = normalVector(start, control, end);

            Handles.color = Color.black;
            Handles.DrawDottedLine(start, control, 2);
            Handles.DrawDottedLine(end, control, 2);
            Handles.color = Color.white;

            Vector3 prev = segment.getPoint(0f);
            for (int idx = 1; idx <= _steps; idx++) {
                Vector3 next = segment.getPoint(idx * stepSize);
                Vector3 prevTransformed = _transform.TransformPoint(prev);
                Handles.DrawLine(prevTransformed, _transform.TransformPoint(next));

                Vector3 tangent = segment.getTangent(idx * stepSize);
                Vector3 tick = Vector3.Cross(normal, tangent).normalized * 0.1f;
                Handles.DrawLine(prevTransformed - tick, prevTransformed + tick);

                prev = next;
            }
        }
    }

    private Vector3 ShowPoint(int index) {
        Vector3 point = _transform.TransformPoint(_curve[index]);
        float size = HandleUtility.GetHandleSize(point);

        EditorGUI.BeginChangeCheck();

        Handles.color = index % 2 == 1 ? Color.red : Color.white;
        if (Handles.Button(point, _rotation, size * _handleSize, size * _pickSize, Handles.CubeCap)) {
            _selected = index;
        }
        
        if (_selected == index) {
            point = Handles.PositionHandle(point, _rotation);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(_curve, "Move point");
                EditorUtility.SetDirty(_curve);
                _curve[index] = _curve.transform.InverseTransformPoint(point);
            }
        }
        return point;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        Path curve = (Path)target;
        if (GUILayout.Button("Add segment")) {
            Undo.RecordObject(curve, "Add segment");
            curve.addSegment();
            EditorUtility.SetDirty(curve);
        }
    }
}
