using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sin : MonoBehaviour
{
    [SerializeField]
    private float h, s, v;
    private float time = 0f;
    [SerializeField]
    private Image image;

    private void FixedUpdate() {
        h = (Mathf.Sin(time+=0.03f)*0.5f)+0.5f;
        s = (Mathf.Cos(time+=0.03f)*0.5f)+0.5f;
        v = (Mathf.Sin(time+=0.03f)*0.5f)+0.5f;

        image.color = new Color(h, s, v, 1f);
    }
}
