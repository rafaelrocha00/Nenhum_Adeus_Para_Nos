using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Tools
{
    public class TextureMipGenerator : MonoBehaviour
    {
        public List<Color> colors = new List<Color>();
        public string fileName = "File";
        public int resolution = 256;
    
        [ContextMenu("Bake Texture")]
        public void GenerateTexture()
        {
            var texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, colors.Count, false);
            print(texture.mipmapCount);
            var textureSize = resolution * resolution;
        
            for (int i = 0; i < colors.Count; i++)
            {
                var pixels = new Color[textureSize];
                for (int j = 0; j < textureSize; j++)
                {
                    pixels[j] = colors[i];
                }
                texture.SetPixels(pixels, i);
            
                textureSize /= 4;
            }
        
            //texture.Apply(); //DO NOT
            AssetDatabase.CreateAsset(texture, $"Assets/Textures/{fileName}.asset");
            AssetDatabase.Refresh();
        }
    }
}
