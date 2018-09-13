using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using iStringLite2DRenderer.Effects;

namespace iStringLite2DRenderer
{
    public static class Utils
    {
        private const string EFFECTS_TAG = "Effects";
        private const string EFFECT_TAG = "Effect";

        /// <summary>
        /// Used for loading hardcoded Effects that does not use the XML document.
        /// </summary>
        /// <param name="effects">The List of Effects to be added to</param>
        public static void SoftLoadEffects(out List<Effect> effects, VideoBuffer videoBuffer)
        {
            effects = new List<Effect>();
            //TODO: Used for testing. This should use the XML file

            //effects.Add(new FillEffect(0, 0, 0));
            effects.Add(new AnimatedGifEffect("Images/gif/rainbow9.gif", 0, 0, videoBuffer));
            //effects.Add(new WaterWhitesEffect(255, 255, 255));
            
            //effects.Add(new FillEffect(0, 40, 180));
            //effects.Add(new ScrollingTextEffect("BLACKPOOL ILLUMINATIONS", 0, 0, 22, 70, Brushes.Red, Brushes.Black, videoBuffer));
            //effects.Add(new BrightnessEffect(0.1));
            //effects.Add(new FillEffect(0, 0, 0)); // clear LEDs
            //effects.Add(new WaterEffect(80, videoBuffer));
            //effects.Add(new AnimatedGifEffect("Images/gif/water4.gif", 0, 0, videoBuffer)); //50, 20, 20));
            //effects.Add(new BitmapEffect("Images/png/water.png", 0, 0, videoBuffer)); //50, 20, 20));
            //effects.Add(new RandomiseEffect(1000));
            //effects.Add(new ScrollingFillEffect(255, 0, 0, 10));
            //effects.Add(new BreathingEffect(0.001, 0.5, 0.01, 10));
        }
        
        /// <summary>
        /// Loads XML file containing a list of Effects and their parameters and
        /// usese reflection to load them into an array of Effects.
        /// </summary>
        /// <param name="effectFileLocation">Location of the effects XML file</param>
        /// <param name="effectList">The array of Effects to write to</param>
        public static void LoadEffects(string effectFileLocation, out Effect[] effectList)
        {
            XDocument effectFile = XDocument.Load(effectFileLocation);
            XNamespace ns = effectFile.Root.GetDefaultNamespace();
            
            var effects = effectFile.Descendants(ns + EFFECTS_TAG).Descendants(ns + EFFECT_TAG);
            effectList = new Effect[effects.Count()];
            int effectIndex = 0;

            Assembly asm = typeof(Renderer).Assembly;

            foreach (var effect in effects)
            {
                string name = effect.Attribute("name")?.Value;
                Object[] parameters = new object[effect.Attributes().Count() - 1]; // create an array to store the parameters in
                
                // store XML attributes in the parameters
                for (int i = 1; i < effect.Attributes().Count(); i++)
                {
                    parameters[i - 1] = effect.Attributes().ElementAt(i).Value;
                }
                
                Type type = asm.GetType(name);
                Object obj = Activator.CreateInstance(type, parameters);
                effectList[effectIndex] = (Effect) obj;
            }
        }
    }
}