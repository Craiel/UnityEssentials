namespace Craiel.UnityEssentials.Profiling
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;

    public class UIFpsGraph : MonoBehaviour
    {
        private readonly List<int> gcCollected = new List<int>();
        private readonly List<int> gcCollectedPosX = new List<int>();
        private readonly List<bool> gcRanThatFrame = new List<bool>();
        private readonly List<Vector3> middlePos = new List<Vector3>();
        private readonly Stopwatch renderTime = Stopwatch.StartNew();
        private readonly List<Vector3> startPos = new List<Vector3>();
        private readonly List<Vector3> stopPos = new List<Vector3>();
        private int currentRefreshRate = 60;
        private long lastGcAmount;
        private Material mat;
        private long memUsed;
        private float renderTimeLastFrame;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public int BottomLeftPosX = 10;

        [SerializeField]
        public int BottomLeftPosY = 40;

//        [SerializeField]
//        public bool DisableVsync = true;

        [SerializeField]
        public int GraphLength = 200;

        [SerializeField]
        public float GraphScale = 10;

        [SerializeField]
        public bool SetTargetFpsToDoubleMonitorHz = false;

        [SerializeField]
        public bool ShowGcCollectedAmount = true;

        public Shader GizmoShader;

        public void Start()
        {
            this.currentRefreshRate = Screen.currentResolution.refreshRate;

//            if (this.DisableVsync)
//            {
//                QualitySettings.vSyncCount = 0;
//            }

            if (this.SetTargetFpsToDoubleMonitorHz)
            {
                Application.targetFrameRate = this.currentRefreshRate * 2;
            }
            else
            {
                Application.targetFrameRate = 2000;
            }

            this.mat = new Material(this.GizmoShader);
        }

        public void OnGUI()
        {

            GUI.Box(new Rect(this.BottomLeftPosX, Screen.height -  this.BottomLeftPosY, this.GraphLength, (1 + this.BottomLeftPosY + 1f / (this.currentRefreshRate * 4) * 1000 * this.GraphScale)),"" );

            UIProfilingUtilities.LabelWithShadow(
                new Rect(this.BottomLeftPosX + this.GraphLength + 5, Screen.height - this.BottomLeftPosY - 10, 100, 20),
                this.memUsed / 1000000 + "Mb");
            UIProfilingUtilities.LabelWithShadow(
                new Rect(this.BottomLeftPosX + this.GraphLength + 5,
                    Screen.height - (1 + this.BottomLeftPosY + 1f / this.currentRefreshRate * 1000 * this.GraphScale) -
                    10, 100,
                    20), this.currentRefreshRate + " fps");
            UIProfilingUtilities.LabelWithShadow(
                new Rect(this.BottomLeftPosX + this.GraphLength + 5,
                    Screen.height -
                    (1 + this.BottomLeftPosY + 1f / (this.currentRefreshRate * 2) * 1000 * this.GraphScale) - 10,
                    100, 20), this.currentRefreshRate * 2 + " fps");
            UIProfilingUtilities.LabelWithShadow(
                new Rect(this.BottomLeftPosX + this.GraphLength + 5,
                    Screen.height -
                    (1 + this.BottomLeftPosY + 1f / (this.currentRefreshRate * 4) * 1000 * this.GraphScale) - 10,
                    100, 20), this.currentRefreshRate * 4 + " fps");

            if (this.ShowGcCollectedAmount)
            {
                for (var i = 0; i < this.gcCollected.Count; i++)
                {
                    var collectedMb = this.gcCollected[i];
                    UIProfilingUtilities.LabelWithShadow(
                        new Rect(this.BottomLeftPosX + this.gcCollectedPosX[i], Screen.height - this.BottomLeftPosY,
                            100, 20),
                        collectedMb + "Mb");
                }
            }
        }

        public void Update()
        {
            while (this.startPos.Count > this.GraphLength)
            {
                this.startPos.RemoveAt(0);
                this.middlePos.RemoveAt(0);
                this.stopPos.RemoveAt(0);
                this.gcRanThatFrame.RemoveAt(0);
            }

            for (var i = this.gcCollected.Count - 1; i >= 0; i--)
            {
                this.gcCollectedPosX[i]--;
                if (this.gcCollectedPosX[i] <= 0)
                {
                    this.gcCollected.RemoveAt(i);
                    this.gcCollectedPosX.RemoveAt(i);
                }
            }


            this.memUsed = GC.GetTotalMemory(false);
            if (this.memUsed < this.lastGcAmount)
            {
                var lastGcRemovedAmount = (int) ((this.lastGcAmount - this.memUsed) / 1000000);
                if (this.ShowGcCollectedAmount)
                {
                    this.gcCollected.Add(lastGcRemovedAmount);
                    this.gcCollectedPosX.Add(this.GraphLength);
                }
                this.gcRanThatFrame.Add(true);
            }
            else
            {
                this.gcRanThatFrame.Add(false);
            }
            this.lastGcAmount = this.memUsed;


            float width = Screen.width;
            float height = Screen.height;

            var msDeltaTime = Time.deltaTime * 1000;

            this.startPos.Add(new Vector3((this.GraphLength + this.BottomLeftPosX + 0.5f) / width,
                (this.BottomLeftPosY + 0.5f) / height));
            this.middlePos.Add(new Vector3((this.GraphLength + this.BottomLeftPosX + 0.5f) / width,
                (this.BottomLeftPosY + 0.5f + (msDeltaTime - this.renderTimeLastFrame) * this.GraphScale) / height));
            this.stopPos.Add(new Vector3((this.GraphLength + this.BottomLeftPosX + 0.5f) / width,
                (this.BottomLeftPosY + 0.5f + msDeltaTime * this.GraphScale) / height));
        }

        public void LateUpdate()
        {
            this.renderTime.Start();
        }

        public IEnumerator OnPostRender()
        {
            yield return new WaitForEndOfFrame();

            float width = Screen.width;
            float heigth = Screen.height;

            GL.PushMatrix();
            this.mat.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.LINES);

            for (var i = 0; i < this.startPos.Count; i++)
            {
                var start = this.startPos[i];
                var middle = this.middlePos[i];
                var stop = this.stopPos[i];

                Color greenColor;
                Color blueColor;
                if (this.gcRanThatFrame[i])
                {
                    blueColor = Color.magenta;
                    greenColor = Color.magenta;
                }
                else
                {
                    blueColor = Color.blue;
                    greenColor = Color.green;
                }

                GL.Color(blueColor);
                GL.Vertex(start);
                GL.Vertex(middle);
                GL.Color(greenColor);
                GL.Vertex(middle);
                GL.Vertex(stop);

                this.startPos[i] = new Vector3(start.x - 1 / width, start.y);
                this.middlePos[i] = new Vector3(middle.x - 1 / width, middle.y);
                this.stopPos[i] = new Vector3(stop.x - 1 / width, stop.y);
            }

            GL.Color(Color.yellow);

            GL.Vertex(new Vector3((this.BottomLeftPosX + 0.5f) / width, (0.5f + this.BottomLeftPosY) / heigth));
            GL.Vertex(new Vector3((this.BottomLeftPosX + this.GraphLength + 1f) / width,
                (0.5f + this.BottomLeftPosY) / heigth));

            GL.Vertex(new Vector3((this.BottomLeftPosX + 0.5f) / width,
                (1.5f + this.BottomLeftPosY + 1f / (this.currentRefreshRate * 4) * 1000 * this.GraphScale) / heigth));
            GL.Vertex(new Vector3((this.BottomLeftPosX + this.GraphLength + 1f) / width,
                (1.5f + this.BottomLeftPosY + 1f / (this.currentRefreshRate * 4) * 1000 * this.GraphScale) / heigth));

            GL.Vertex(new Vector3((this.BottomLeftPosX + 0.5f) / width,
                (1.5f + this.BottomLeftPosY + 1f / (this.currentRefreshRate * 2) * 1000 * this.GraphScale) / heigth));
            GL.Vertex(new Vector3((this.BottomLeftPosX + this.GraphLength + 1f) / width,
                (1.5f + this.BottomLeftPosY + 1f / (this.currentRefreshRate * 2) * 1000 * this.GraphScale) / heigth));

            GL.Vertex(new Vector3((this.BottomLeftPosX + 0.5f) / width,
                (1.5f + this.BottomLeftPosY + 1f / this.currentRefreshRate * 1000 * this.GraphScale) / heigth));
            GL.Vertex(new Vector3((this.BottomLeftPosX + this.GraphLength + 1f) / width,
                (1.5f + this.BottomLeftPosY + 1f / this.currentRefreshRate * 1000 * this.GraphScale) / heigth));

            GL.End();
            GL.PopMatrix();

            this.renderTime.Stop();
            this.renderTimeLastFrame = (float) this.renderTime.Elapsed.TotalMilliseconds;
            this.renderTime.Reset();
        }
    }
}