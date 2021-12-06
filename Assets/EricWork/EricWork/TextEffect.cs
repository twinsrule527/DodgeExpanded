using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    public TMP_Text textmesh;
    public Mesh mesh;
    public Vector3[] vertices1;

    public float sineSET;
    public float cosSet;

    public float timeLapse = 0.07f;
    public float spaceLapse = 0.7f;

    //public string str;
    // Start is called before the first frame update
    void Start()
    {
        textmesh = GetComponent<TMP_Text>();
        //str = textmesh.text;
        //StartCoroutine(BuildText(textmesh, "SHAMALAYA BANG. BANG? ERIC TEST"));
        
        

    }

    // Update is called once per frame
    void Update()
    {


        textmesh.ForceMeshUpdate();
        mesh = textmesh.mesh;
        vertices1 = mesh.vertices;



        for (int i = 0; i < vertices1.Length; i++)
        {
            Vector3 offset = Wobble(Time.time + i);
            vertices1[i] = vertices1[i] + offset;
        }

        mesh.vertices = vertices1;
        textmesh.canvasRenderer.SetMesh(mesh);
    }

    Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * sineSET), Mathf.Cos(time * cosSet));
    }

    public IEnumerator BuildText(TMP_Text ts, string str = "")
    {
        ts.text = "";
        for (int i = 0; i < str.Length; i++)
        {
            if (i > 0)
            {
                if (str[i - 1] == '.' || str[i - 1] == '?')
                {
                    yield return new WaitForSeconds(spaceLapse);
                }
            }
            ts.text = string.Concat(ts.text, str[i]);
            //Wait a certain amount of time, then continue with the for loop
            yield return new WaitForSeconds(timeLapse);
        }
    }
}
