using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CaseDataObjects 
{
    [Serializable]
    public struct CaseProperty
    {
        public string propertyValue;
        
        [Tooltip("If true, this value will be known to player at start of new case.")]
        public bool startAsDiscovered;
    }

    [Serializable]
    public struct CaseImage
    {
        public Sprite portraitSprite;
        
        [Tooltip("If true, this value will be known to player at start of new case.")]
        public bool startAsDiscovered;
    }

    [Serializable]
    public struct CaseTheory
    {
        [Tooltip("Evidence to present for this theory.")]
        public CaseEvidence evidence;
        
        [Tooltip("Resposne the courts will provide as to validity of evidencial theory.")]
        public string response;
    }

    [Serializable]
    public struct SuspectTheory
    {
        public CaseTheory[] ValidTheories;
        public CaseTheory[] InvalidTheories;
    }
}
