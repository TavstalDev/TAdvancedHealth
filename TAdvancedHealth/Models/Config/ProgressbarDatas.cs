﻿using System.Collections.Generic;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class ProgressBarDatas
    {
        public float LastFood = 0;
        public float LastWater = 0;
        public float LastStamina = 0;
        public float LastVirus = 0;
        public float LastOxygen = 0;
        public float LastSimpleHealth = 0;
        public float LastHealthHead = 0;
        public float LastHealthBody = 0;
        public float LastHealthLeftArm = 0;
        public float LastHealthLeftLeg = 0;
        public float LastHealthRightArm = 0;
        public float LastHealthRightLeg = 0;
        public List<string> VisibleSimpleHealth = new List<string>();
        public List<string> VisibleFood = new List<string>();
        public List<string> VisibleWater = new List<string>();
        public List<string> VisibleVirus = new List<string>();
        public List<string> VisibleStamina = new List<string>();
        public List<string> VisibleOxygen = new List<string>();
        public List<string> VisibleHead = new List<string>();
        public List<string> VisibleBody = new List<string>();
        public List<string> VisibleLeftArm = new List<string>();
        public List<string> VisibleLeftLeg = new List<string>();
        public List<string> VisibleRightArm = new List<string>();
        public List<string> VisibleRightLeg = new List<string>();

        public ProgressBarDatas() { }

        public ProgressBarDatas(float lastFood, float lastWater, float lastStamina, float lastVirus, float lastOxygen, float lastSimpleHealth, float lastHealthHead, float lastHealthBody, float lastHealthLeftArm, float lastHealthLeftLeg, float lastHealthRightArm, float lastHealthRightLeg, List<string> visibleSimpleHealth, List<string> visibleFood, List<string> visibleWater, List<string> visibleVirus, List<string> visibleStamina, List<string> visibleOxygen, List<string> visibleHead, List<string> visibleBody, List<string> visibleLeftArm, List<string> visibleLeftLeg, List<string> visibleRightArm, List<string> visibleRightLeg)
        {
            LastFood = lastFood;
            LastWater = lastWater;
            LastStamina = lastStamina;
            LastVirus = lastVirus;
            LastOxygen = lastOxygen;
            LastSimpleHealth = lastSimpleHealth;
            LastHealthHead = lastHealthHead;
            LastHealthBody = lastHealthBody;
            LastHealthLeftArm = lastHealthLeftArm;
            LastHealthLeftLeg = lastHealthLeftLeg;
            LastHealthRightArm = lastHealthRightArm;
            LastHealthRightLeg = lastHealthRightLeg;
            VisibleSimpleHealth = visibleSimpleHealth;
            VisibleFood = visibleFood;
            VisibleWater = visibleWater;
            VisibleVirus = visibleVirus;
            VisibleStamina = visibleStamina;
            VisibleOxygen = visibleOxygen;
            VisibleHead = visibleHead;
            VisibleBody = visibleBody;
            VisibleLeftArm = visibleLeftArm;
            VisibleLeftLeg = visibleLeftLeg;
            VisibleRightArm = visibleRightArm;
            VisibleRightLeg = visibleRightLeg;
        }
    }
}