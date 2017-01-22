using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VerticalSwipeHelper
{

    Dictionary<int, float> _touchDownPosition = new Dictionary<int, float>();

    public void Reset()
    {
        _touchDownPosition.Clear ();
    }

    public int UpdateAxisValue()
    {
        var touches = new List<Touch>(Input.touches);

        if(Input.GetMouseButtonDown(1))
        {
            var newTouch = new Touch ();
            newTouch.phase = TouchPhase.Began;
            newTouch.position = Input.mousePosition;
            newTouch.fingerId = -1;
            touches.Add (newTouch);
        }

        if(Input.GetMouseButtonUp(1))
        {
            var newTouch = new Touch ();
            newTouch.phase = TouchPhase.Ended;
            newTouch.position = Input.mousePosition;
            newTouch.fingerId = -1;
            touches.Add (newTouch);
        }

        int axisValue = 0;

        for (int i = 0, count = touches.Count; i < count; i++) {
            var currentTouch = touches [i];
            switch (currentTouch.phase) {
            case TouchPhase.Began:
                {
                    _touchDownPosition [currentTouch.fingerId] = currentTouch.position.y;
                    break;
                }

            case TouchPhase.Ended:
                {
                    var delta = currentTouch.position.y - _touchDownPosition [currentTouch.fingerId];
                    if (!Mathf.Approximately(delta, 0)) 
                    {
                        axisValue = delta > 0 ? 1 : -1;
                    }
                    _touchDownPosition.Remove (currentTouch.fingerId);
                    break;
                }
            }
        }
        return axisValue;
    }
}

