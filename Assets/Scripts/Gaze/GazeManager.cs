using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using HoloToolkit.Unity.InputModule;
using M1.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

//For animating screen cursor, main functions to be used are GazeTo() and ResetGaze()
//Attach the script GazeTrigger.cs to a gameobject with a box collider and it will call these
//Automatically through the use of OnGazeEnter() and OnGazeLeave()
//Also uses object cursor to keep crosshair in middle of screen
//-Andrew
public class GazeManager : SingletonBehaviour<GazeManager>
{
    public Image cursorVisual;
    public ObjectCursor cursor;
    public Transform[] corners = new Transform[0];

    public AnimationCurve moveCurve;
    public float easeTime = 0.25f;

    private Vector3[] defaultCorners = new Vector3[4];
    private Quaternion storedRotation;
    private bool isGazing = false;
    public static bool canGaze = false;

    public delegate void CallBack();

    internal GameObject currentGazeTarget;
    public GameObject gazeStart;

    private IEnumerator Start()
    {
        canGaze = false;
        yield return null;
        DelayGazeInput(3f);
    }


    public void DelayGazeInput(float delay)
    {
        StartCoroutine(iDelayGazeInput(delay));
    }

    private IEnumerator iDelayGazeInput(float delay)
    {
        canGaze = false;
        yield return new WaitForSeconds(delay);

        canGaze = true;
        gazeStart.GetComponent<Image>().enabled = false;
        yield return null;
        gazeStart.GetComponent<Image>().enabled = true;

    }

    #region Core

    private float elapsed = 0f;
    private float interval = 0.8f;
    private float scalar = 0f;
    private bool initProgress = false;

    public GameObject currentGazeObject;

    private void Update()
    {

        if (!isGazing)
        {
            RaycastHit hit;

            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hit,
                Mathf.Infinity))
            {
                if (hit.transform.name == "Exit")
                {
                    currentGazeObject = hit.transform.gameObject;
                    hit.transform.gameObject.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
                }
                else if (hit.transform.name != "Exit")
                {
                    if (currentGazeObject != null)
                    {
                        hit.transform.gameObject.SendMessage("OnGazeLeave", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }

        TrackGazeTime();
    }

    public void GazeTo(GameObject target, bool trackProgress = true)
    {
        isGazing = trackProgress;

        currentGazeTarget = target;
        GazeToReset();

        BoxCollider col = target.GetComponent<BoxCollider>();

        if (col == null)
        {
            Debug.LogError("GazeTo() - Parameter ~target~ has no collider");
            return;
        }
    }

    public void GazeOff()
    {
        GazeOffReset();
    }

    private void TrackGazeTime()
    {
        //Track amount of time user gazes at Gazeable object
        if (isGazing)
        {
            elapsed += Time.deltaTime / interval;
            scalar = elapsed * 1f;

            //Delay before progress meter starts filling
            if (scalar >= 0.25f && !initProgress)
            {
                initProgress = true;
                Debug.Log(".25f scale");

                cursorVisual.GetComponent<CanvasGroup>().alpha = 1f;
                scalar = 0f;
                elapsed = 0f;
            }
            else if (!initProgress) return;

            cursorVisual.fillAmount = scalar;

            //Once progress meter has completed fill
            if (scalar >= 1.25f)
            {
                OnGazeComplete();
                isGazing = false;
            }
        }
    }

    #endregion

    #region Reset

    private void GazeToReset()
    {
        cursorVisual.GetComponent<CanvasGroup>().interactable = true;
        initProgress = false;
        isGazing = true;
        scalar = 0f;
        elapsed = 0f;
    }

    private void GazeOffReset()
    {
        isGazing = false;

        cursorVisual.fillAmount = 0f;
        if (cursorVisual.GetComponent<CanvasGroup>().interactable)
            FadeManager.FadeOut(cursorVisual.GetComponent<CanvasGroup>(), 0.5f);

    }

    #endregion

    #region Events

    private void OnResetComplete()
    {

    }

    private void OnGazeComplete()
    {
        Debug.Log("OnGazeComplete()");

        StartCoroutine(DelayFade());
        currentGazeTarget.SendMessage("OnGazeComplete", SendMessageOptions.DontRequireReceiver);
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(1);
        FadeManager.FadeOut(cursorVisual.GetComponent<CanvasGroup>(), 1f);
    }

    #endregion

    #region Collider Math Helper Functions

    private Vector3[] GetColliderVertexPositions(GameObject target, BoxCollider col)
    {
        float offset = 1f;

        var vertices = new Vector3[8];
        var thisMatrix = target.transform.localToWorldMatrix;
        var storedRotation = target.transform.rotation;
        target.transform.rotation = Quaternion.identity;

        var extents = col.size/2.0f;
        vertices[0] = thisMatrix.MultiplyPoint3x4(extents*offset);
        vertices[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z)*offset);
        vertices[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z)*offset);
        vertices[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z)*offset);
        vertices[4] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z)*offset);
        vertices[5] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z)*offset);
        vertices[6] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z)*offset);
        vertices[7] = thisMatrix.MultiplyPoint3x4(-extents*offset);

        target.transform.rotation = storedRotation;
        return vertices;
    }

    #endregion

    #region Animate Helper Functions

    private void MoveTo(Transform _rectTransfrom, float _time, Vector3 _from, Vector3 _to, AnimationCurve _curve, CallBack cb = null, bool local = false)
    {
        //StartCoroutine(iMoveTo(_rectTransfrom, _time, _from, _to, _curve, cb, local));
    }

    private IEnumerator iMoveTo(Transform _rectTransfrom, float _time, Vector3 _from, Vector3 _to, AnimationCurve _curve, CallBack cb, bool local)
    {
        float t = 0;
        while (t < _time)
        {
            yield return null;
            t += Time.deltaTime;
            float delta = _curve.Evaluate(Mathf.LerpUnclamped(0.0f, 1.0f, t / _time));

            if (local)
                _rectTransfrom.localPosition = Vector3.LerpUnclamped(_from, _to, delta);
            else
                _rectTransfrom.position = Vector3.LerpUnclamped(_from, _to, delta);

        }

        if (local)
            _rectTransfrom.localPosition = _to;
        else
            _rectTransfrom.position = _to;


        if (cb != null)
        {
            cb();
        }
    }

    #endregion
}
