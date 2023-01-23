using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakersWrath.Saving {

    public class TimedFunctionCall {
        private bool triggered = false;
        public TimedFunctionCall(Func<bool> _f, float _timeToCall) 
        {
            f = _f;
            timeToCall = _timeToCall;
        }
        public void SetTriggered(bool status) { triggered = status; }
        public bool IsTriggered() { return triggered; }
        public Func<bool> f { get; }
        public float timeToCall { get; }
    }

    public readonly struct SaveableClockState {
        public SaveableClockState(float _time, List<TimedFunctionCall> _tasks) {
            time = _time;
            tasks = _tasks;
        }
        public float time { get; }
        public List<TimedFunctionCall> tasks { get; }
    }

    // This should be used for saveable timeframes
    // NOTE: will need to change implementation+design to support stuff similar to wait().
    public class SaveableClock : MonoBehaviour, ISaveable
    {
        float time = 0;
        List<TimedFunctionCall> tasks = new List<TimedFunctionCall>();

        void Update() {
            // List<TimedFunctionCall> tasksCopy = this.DeepCopyTasks();
            for (int i = 0; i < tasks.Count; i++) {
                TimedFunctionCall task = tasks[i];
                if (task.timeToCall <= this.time && !task.IsTriggered()) {
                    task.f();
                    task.SetTriggered(true);
                }
            }
            // tasks = tasksCopy;
            this.time += Time.deltaTime;
        }

        public void AddTimedFunctionCall(Func<bool> f, float timeToCall) {
            tasks.Add(new TimedFunctionCall(f, timeToCall + this.time));
        }

        public object CaptureState()
        {  
            return new SaveableClockState(this.time, DeepCopyTasks());
        }

        List<TimedFunctionCall> DeepCopyTasks() {
            List<TimedFunctionCall> newTasks = new List<TimedFunctionCall>();
            foreach (TimedFunctionCall task in tasks) {
                TimedFunctionCall newTask = new TimedFunctionCall(task.f, task.timeToCall);
                newTask.SetTriggered(task.IsTriggered());
                newTasks.Add(newTask);
            }
            return newTasks;
        }

        public void RestoreState(object state) 
        {
            this.time = ((SaveableClockState) state).time;
            this.tasks = ((SaveableClockState) state).tasks;
        }

    }
}
