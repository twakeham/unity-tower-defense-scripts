using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Component : Makes a Bezier path with a given number of segments

[System.Serializable]
public class Segment {
    /// Represents a single segment in the path

    [SerializeField]
    private Vector3 _start;
    [SerializeField]
    private Vector3 _control;
    [SerializeField]
    private Vector3 _end;

    public Segment previous;
    public Segment next;

    public Segment() {
        _start = new Vector3(3, 0, 0);
        _control = new Vector3(4, 0, 0);
        _end = new Vector3(5, 0, 0);
    }

    public Segment(Vector3 start, Vector3 control, Vector3 end) {
        _start = start;
        _control = control;
        _end = end;
    }

    public Vector3 getPoint(float t) {
        return Vector3.Lerp(Vector3.Lerp(start, control, t), Vector3.Lerp(control, end, t), t);
    }

    public Vector3 getTangent(float t) {
        return 2f * (1f - t) * (control - start) + 2f * t * (end - control);
    }

    public Vector3 start {
        get {
            return _start;
        }
        set {
            if (previous != null) previous.end = value; 
            _start = value;
        }
    }

    public Vector3 control {
        get {
            return _control;
        }
        set {
            _control = value;
        }
    }

    public Vector3 end {
        get {
            if (next != null) {
                return next.start;
            } else {
                return _end;
            }
        }
        set {
            _end = value;
        }
    }
}


public class Path : MonoBehaviour {

    public int pathAccuracy = 20;
    public List<Segment> segments = new List<Segment>();

    public Segment addSegment() {
        Segment newSegment;
        if (segments.Count == 0) {
            newSegment = new Segment();
        } else {
            Segment lastSegment = segments[segments.Count - 1];
            Vector3 dir = (lastSegment.end - lastSegment.control).normalized;
            newSegment = new Segment(lastSegment.end, lastSegment.end + dir, lastSegment.end + dir * 2);
            newSegment.previous = lastSegment;
            lastSegment.next = newSegment;
        }
        segments.Add(newSegment);
        return newSegment;
    }

    public Vector3 getPoint(float t) {
        int segmentIdx = (int)t;
        Segment segment = segments[segmentIdx];
        t = t - segmentIdx;
        return transform.TransformPoint(segment.getPoint(t));
    }

    public Vector3 getTangent(float t) {
        int segmentIdx = (int)t;
        Segment segment = segments[segmentIdx];
        t = t - segmentIdx;
        return segment.getTangent(t);
    }


    public int Length {
        /// Length is number of handles
        get {
            return segments.Count != 0 ? segments.Count * 2 + 1 : 0;
        }
    }

    public Vector3 this[int index] {
        // get segment at index
        get { 
            //Debug.Log(index);
            if (index == Length - 1) {
                return segments[segments.Count - 1].end;
            } else {
                int segmentIndex = index / 2;
                Segment segment = segments[segmentIndex];
                if (index % 2 == 0) {
                    return segment.start;
                } else {
                    return segment.control;
                }
            }
        }
        set {
            if (index == Length - 1) {
                segments[segments.Count - 1].end = value;
            } else {
                int segmentIndex = index / 2;
                Segment segment = segments[segmentIndex];
                if (index % 2 == 0) {
                    segment.start = value;
                } else {
                    segment.control = value;
                }
            }
        }
    }


}
