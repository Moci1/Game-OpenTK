using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using Entities;
using System.Drawing;

namespace Animation
{
    public enum AnimBehavior
    {
        HoldEnd, Reverse, Replay
    }
	public interface IAnimBorder
    {
        // string név a soroknak -> filter of the BindingList
        int FrameRowCount { get; }
        int FrameWidth { get; } // ActualWidth is kéne
        int FrameHeight { get; }
        Rectangle this[ushort index] { get; } // x = (index % rowW) * W, y = (index / rowW) * H
        Rectangle this[sbyte row, sbyte column] { get; }
        Rectangle FromPosition(sbyte x, sbyte y);
        ushort FromRectangle(Rectangle? r);
    }
    public class AnimPlayer
    {
		Frame frame;
        IAnimBorder iAnim;
        Dictionary<string, List<ushort>> anims = new Dictionary<string, List<ushort>>();
        Timer timer;
        ushort current;

        public AnimPlayer(Frame frame, IAnimBorder resource)
        {
			this.frame = frame;
            this.iAnim = resource;
            current = resource.FromRectangle(frame.SourceRectangle);
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }
        public void CreateAnim(string name, List<ushort> indices)
        {
            List<ushort> list = indices;
            anims.Add(name, list);
            if (anims.Count == 1)
                Animation = name;
            //indices = null; csakhogy nehogy össze legyen kötve a list és az indices referencia szerint!
        }
        public void CreateAnim(string name, params ushort[] indices)
        {
            //anims.Add(name, indices);
            if (anims.Count == 1)
                Animation = name;
            //indices = null; csakhogy nehogy össze legyen kötve a list és az indices referencia szerint!
        }
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            frame.SourceRectangle = iAnim[anims[Animation][current]];
            if (current < anims[Animation].Count - 1)
                current++;
            else if (Behavior == AnimBehavior.Replay)
                current = 0;
            else if (Behavior == AnimBehavior.Reverse)
                current--;
            else if (Behavior == AnimBehavior.HoldEnd)
                timer.Stop();
        }
        public AnimBehavior Behavior { get; set; }
        public void Play()
        {
            frame.SourceRectangle = iAnim[anims[Animation][current]];
            if (current < anims[Animation].Count - 1)
                current++;
            timer.Start();
        }
        public void Play(string animName)
        {
            Animation = animName;
            frame.SourceRectangle = iAnim[anims[animName][current]];
            if (current < anims[Animation].Count - 1)
                current++;
            timer.Start();
        }
        bool pause;
        public bool Pause
        {
            get
            {
                return pause;
            }
            set
            {
                pause = value;
                if (pause)
                    timer.Stop();
                else
                    Play();
            }
        }
        bool stop;
        public bool Stop
        {
            get
            {
                return stop;
            }
            set
            {
                stop = value;
                if (stop)
                {
                    timer.Stop();
                    current = 0;
                    frame.SourceRectangle = iAnim[anims[Animation][current]];
                }
                else
                {
                    current = 0;
                    frame.SourceRectangle = iAnim[anims[Animation][current]];
                    Play();
                }
            }
        }
        public string Animation { get; set; }
        public ushort Current
        {
            get
            {
                return current;
            }
        }
        public double RefreshRate
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }
    }
}