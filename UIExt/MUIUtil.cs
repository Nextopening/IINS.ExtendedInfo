using System.IO;
using System.Reflection;
using UnityEngine;


namespace IINS.UIExt
{
    public static class MUIUtil 
    {
  
        // 取随机颜色
        public static Color GetRandomColor(float alpha = 1.0f)
        {
            float r = UnityEngine.Random.Range(0.1f, 0.95f);
            float g = UnityEngine.Random.Range(0.1f, 0.95f);
            float b = UnityEngine.Random.Range(0.1f, 0.95f);
            return new Color(r, g, b, alpha);
        }

        // 颜色加亮
        public static Color BrightenColor(Color C, float Percentage, float alpha = 1.0f)
        {
            Percentage = 1.0f - Percentage;
            Percentage = Percentage > 0.0f ? Percentage : 0.1f;
            Percentage = Percentage < 1.0f ? Percentage : 0.9f;

            float r = 1.0f - Percentage * (1.0f - C.r); 
            float g = 1.0f - Percentage * (1.0f - C.g); 
            float b = 1.0f - Percentage * (1.0f - C.b); 
            return new Color(r, g, b, alpha);
        }

        // 颜色变暗
        public static Color DarkenColor(Color C, float Percentage, float alpha = 1.0f)
        {
            Percentage = 1.0f - Percentage;
            Percentage = Percentage > 0.0f ? Percentage : 0.1f;
            Percentage = Percentage < 1.0f ? Percentage : 0.9f;

            float r = Percentage * (C.r); 
            float g = Percentage * (C.g); 
            float b =  Percentage * (C.b); 
            return new Color(r, g, b, alpha);
        }
         
        // 从资源中读取贴图
        private static Stream loadAssemblyTexture(Assembly assembly, string file)
        {
            Stream st = null;
            if (assembly != null && file != "")
            {
                st = assembly.GetManifestResourceStream(file);
                if (st==null)
                    st = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + file);
                if (st == null)
                    st = assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources." + file);                
            }

            return st;
        }

         public static Texture2D loadTextureFromAssembly(string file, int width = 2, int height = 2)
        {
            Texture2D texture = null;
            Assembly rootAssembly = Assembly.GetCallingAssembly();
            texture = loadTextureFromAssembly(rootAssembly, file, width, height);

            if (texture == null && rootAssembly != Assembly.GetExecutingAssembly())
                texture = loadTextureFromAssembly(Assembly.GetExecutingAssembly(), file, width, height);
            return texture;
        }

        // like assembly = Assembly.GetAssembly(typeof(MUIUtil))
        public static Texture2D loadTextureFromAssembly(Assembly assembly, string file, int width, int height)
        {
            Texture2D texture = null;

            using (Stream st = loadAssemblyTexture(assembly, file))
            {
                if (st != null)
                {
                    byte[] bytes = new byte[st.Length];
                    st.Read(bytes, 0, bytes.Length);

                    texture = new Texture2D(width, height, TextureFormat.ARGB32, false)
                    {
                        filterMode = FilterMode.Bilinear,
                    };

                    texture.LoadImage(bytes);
                    texture.Apply(true, false);
                }

            }

            return texture;
        }

        //public static Texture2D loadTextureFromAssembly(string assemblyName, string file, int width = 2, int height = 2)
        //{

        //}

        // 从文件中读取贴图
        public static Texture2D loadTextureFromFile(string file, int width = 2, int height = 2)
        {
            Texture2D tempTexture = null;

            if (File.Exists(file))
            {
                byte[] bytes = File.ReadAllBytes(file);
                tempTexture = new Texture2D(width, height, TextureFormat.ARGB32, false)
                {
                    filterMode = FilterMode.Bilinear,
                };

                tempTexture.LoadImage(bytes);
                tempTexture.Apply(true, false);
                
            }

            return tempTexture;
        }     
    }
}
