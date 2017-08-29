using UnityEngine;
using System.Collections;
using M1.Utilities;

public class FadeManager : SingletonBehaviour<FadeManager>
{

    public static void FadeIn(CanvasGroup c, float _time, float _alpha = 1f)
    {
        Instance.StartCoroutine(Instance.iFadeIn(c, _time, _alpha));
    }

    IEnumerator iFadeIn(CanvasGroup objToFade, float _time, float _alpha = 1f)
    {
        while (objToFade.alpha < _alpha)
        {
            objToFade.alpha += Time.deltaTime / _time;
            yield return null;
        }

        objToFade.alpha = _alpha;
    }

    public static void FadeOut(CanvasGroup c, float _time, float _alpha = 0f)
    {
        Instance.StartCoroutine(Instance.iFadeOut(c, _time, _alpha));
    }

    IEnumerator iFadeOut(CanvasGroup objToFade, float _time, float _alpha = 0f)
    {
        while (objToFade.alpha > _alpha)
        {
            objToFade.alpha -= Time.deltaTime / _time;
            yield return null;
        }

        objToFade.alpha = _alpha;
    }

    public static void FadeIn(Material m, float _time, float _alpha = 1f)
    {
        Instance.StartCoroutine(Instance.iFadeIn(m, _time, _alpha));
    }

    IEnumerator iFadeIn(Material objToFade, float _time, float _alpha = 1f)
    {
        while (objToFade.color.a < _alpha)
        {
            objToFade.color = new Color(objToFade.color.r, objToFade.color.g, objToFade.color.b, objToFade.color.a + (Time.deltaTime / _time));
            yield return null;
        }

        objToFade.color = new Color(objToFade.color.r, objToFade.color.g, objToFade.color.b, _alpha);
    }

    public static void FadeOut(Material m, float _time, float _alpha = 0f)
    {
        Instance.StartCoroutine(Instance.iFadeOut(m, _time, _alpha));
    }

    IEnumerator iFadeOut(Material objToFade, float _time, float _alpha = 0f)
    {
        while (objToFade.color.a > _alpha)
        {
            objToFade.color = new Color(objToFade.color.r, objToFade.color.g, objToFade.color.b, objToFade.color.a - (Time.deltaTime / _time));
            yield return null;
        }

        objToFade.color = new Color(objToFade.color.r, objToFade.color.g, objToFade.color.b, _alpha);
    }
}
