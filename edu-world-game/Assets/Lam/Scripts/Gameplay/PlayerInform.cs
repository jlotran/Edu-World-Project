    using System;
    using UnityEngine;
    using TMPro;


    namespace Lam.GAMEPLAY
    {   
        public enum LevelType
        {
            Intern,
            Fresher,
            Junior,
            Middle,
            Senior,
            Lead,
            Manager,
            Director,
            CEO,
        }
        public enum JobType
        {
            Developer,
            Designer,
            QA,
            Manager,
        }
        public class PlayerInform : MonoBehaviour
        {
            [SerializeField] private TMP_Text playerName;
            [SerializeField] private TMP_Text playerLevel;

            public void SetData(string name, LevelType level, JobType job)
            {
                playerName.text = name;
                playerLevel.text = $"{level.ToString()} - {job.ToString()}";
            }

            private void Start()
            {
                SetData(Util.RandomName(), GetLevel(), GetJob());
            }

            private LevelType GetLevel()
            {
                Array values = Enum.GetValues(typeof(LevelType));
                return (LevelType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
            }

            private JobType GetJob()
            {
                Array values = Enum.GetValues(typeof(JobType));
                return (JobType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
            }
        }
    }
