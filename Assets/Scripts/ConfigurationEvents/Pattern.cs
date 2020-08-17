﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An event that we can schedule to perform via a PatternMeasure.
/// Has defined behaviors for its collection of PatternActions. 
/// </summary>
public abstract class Pattern : MonoBehaviour {
    [Serializable]
    public class PatternAction {
        [HideInInspector]
        public int ID;
        public string ActionName;
        public UnityEvent OnPatternAction;
    }
    
    public static readonly string NoneString = "None";
    private static PatternAction NoneAction = new PatternAction {
        ActionName = NoneString, OnPatternAction = null
    };
    // private PatternAction SpawnAction = new PatternAction {
    //     ActionName = "Spawn", OnPatternAction  = new UnityEvent().AddListener(delegate { SpawnPattern(); })
    // };
    
    [SerializeField]
    private List<PatternAction> PatternActions;

    public void InvokePatternAction(int id) {
        PatternActions.First(e => e.ID == id).OnPatternAction.Invoke();
    }

    public PatternAction[] GetAllPatternActions() {
        List<PatternAction> allPatternActions = new List<PatternAction>(PatternActions); 
        allPatternActions.Insert(0, NoneAction);
        return allPatternActions.ToArray();
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(Pattern), true)]
    public class PatternEditor : Editor {

        public override void OnInspectorGUI() {
            // Draw the default inspector
            DrawDefaultInspector();
            Pattern pattern = target as Pattern;
            
            int i = 0;
            pattern.PatternActions.ForEach(e => e.ID = i++);
        }
    }
    #endif
}