using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/BorderLevelScriptableObject")]
public class BorderLevelScriptableObject : ScriptableObject
{
    public List<Border> Borders;
    public List<BorderTransition> BorderTransitions;
    public List<float> transitionTimes;
    [SerializeField] private List<BorderTool> BorderCreation;//A list of border tools used for this level
    public void SetBorderTransitions(List<float> timeChanges) {
        List<BorderTransition> transList = new List<BorderTransition>();
        for(int i = 0; i < timeChanges.Count; i++) {
            if(i < Borders.Count - 1) {
                BorderTransition newTrans = new BorderTransition();
                newTrans.changeTime = timeChanges[i];
                newTrans.startBorder = Borders[i];
                newTrans.endBorder = Borders[i+1];
                transList.Add(newTrans);
            }
        }
        BorderTransitions =  transList;
    }

    public void SetBordersFromTool() {
        List<BorderTool> BorderCreationUnordered = new List<BorderTool>(FindObjectsOfType<BorderTool>());
        //Orders BorderCreation by their number
        BorderCreation = new List<BorderTool>(BorderCreationUnordered);
        for(int i = 0; i < BorderCreationUnordered.Count; i++) {
            BorderCreation[BorderCreationUnordered[i].Number] = BorderCreationUnordered[i];
            BorderCreationUnordered[i].gameObject.SetActive(false);
        }
        for(int i = 0; i < BorderCreation.Count; i++) {
            Borders.Add(BorderCreation[i].CreateBorderFromThis());
        }
    }
}
