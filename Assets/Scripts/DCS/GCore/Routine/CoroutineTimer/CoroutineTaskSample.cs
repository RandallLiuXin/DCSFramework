using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Timer
{
    public class CoroutineTaskSample
    {
        public void Execute()
        {
            CoroutineTask task = new CoroutineTask(this, CallWrapper(), OnFinish);

            task.Start();
            task.Pause();
            task.Unpause();
            task.Stop();

            // 注：基于Mono的task不用关心销毁， 非Mono要再不用的时候手动Stop();
        }

        private void OnFinish(bool finish)
        {
            if (finish)
            {
                // 走完
            }
            else
            {
                // 未处理完就Stop()了
            }
        }

        IEnumerator CallWrapper()
        {
            yield return new WaitForEndOfFrame();
            //yield return Yielders.WaitEndOfFrame;
        }
    }
}
