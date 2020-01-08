using UnityEngine;
using ColossalFramework.UI;
using System.Reflection;

namespace IINS.UIExt
{
    public class MUITextureAtlas
    {
        private static UITextureAtlas m_atlas;
        public static UITextureAtlas atlas { get { return m_atlas; } }

        public static bool isAtlasExists(string name)
        {
            return ((atlas == null || name == "") ? false: atlas[name]!=null);
        }

        public static void releaseStaticAtlas()
        {
            if (atlas != null)
                UnityEngine.Object.Destroy(atlas);
        }

        private static void createStaticAtlas()
        {
            if (m_atlas != null) return;

            int maxSize = 1024;
            Texture2D texture2D = new Texture2D(maxSize, maxSize, TextureFormat.ARGB32, false);
            m_atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            Material material = UnityEngine.Object.Instantiate<Material>(UIView.GetAView().defaultAtlas.material);
            material.mainTexture = texture2D;
            m_atlas.material = material;
            m_atlas.name = "IINS.textureAtlas";
        }

        // 从程序资源中添加资源
        public static UITextureAtlas AddTextureAtlasFromAssembly(Assembly assembly, string[] spriteNames, string assemblyPath)
        {
            createStaticAtlas();

            Texture2D[] textures = new Texture2D[spriteNames.Length];
            

            for (int i = 0; i < spriteNames.Length; i++)
            {
                textures[i] = MUIUtil.loadTextureFromAssembly(assembly, assemblyPath + spriteNames[i] + ".png", 2, 2);
                textures[i].name = spriteNames[i];
            }

            AddTextures(textures);

            //Rect[] regions = new Rect[spriteNames.Length]; 
            //regions = ((Texture2D)(m_atlas.material.mainTexture)).PackTextures(textures, 2, 1024);

            //for (int i = 0; i < spriteNames.Length; i++)
            //{
            //    UITextureAtlas.SpriteInfo item = new UITextureAtlas.SpriteInfo
            //    {
            //        name = spriteNames[i],
            //        texture = textures[i],
            //        region = regions[i],
            //    };

            //    m_atlas.AddSprite(item);
            //}

            //m_atlas.RebuildIndexes();
            return m_atlas;
        }

        // 从文件中添加资源
        public static UITextureAtlas AddTextureAtlasFromFile(string[] spriteNames, string Path)
        {
            createStaticAtlas();

            Texture2D[] textures = new Texture2D[spriteNames.Length];
            

            for (int i = 0; i < spriteNames.Length; i++)
            {
                textures[i] = MUIUtil.loadTextureFromFile(Path + spriteNames[i] + ".png");
                textures[i].name = spriteNames[i];
            }

            AddTextures(textures);

            //Rect[] regions = new Rect[spriteNames.Length];
            //regions = ((Texture2D)(m_atlas.material.mainTexture)).PackTextures(textures, 2, 1024);

            //for (int i = 0; i < spriteNames.Length; i++)
            //{
            //    UITextureAtlas.SpriteInfo item = new UITextureAtlas.SpriteInfo
            //    {
            //        name = spriteNames[i],
            //        texture = textures[i],
            //        region = regions[i],
            //    };

            //    m_atlas.AddSprite(item);
            //}

            return m_atlas;
        }

        // 添加游戏中默认的资源
        public static void AddDefaultTextures(string[] spriteNames)
        {
            createStaticAtlas();

            int iCount = 0;
            int[] spriteIndexs = new int[spriteNames.Length];

            for (int i = 0; i < spriteNames.Length; i++)
                if (m_atlas[spriteNames[i]] == null)
                {
                    spriteIndexs[i] = i;
                    iCount++;
                }
                else
                    spriteIndexs[i] = -1;

            Texture2D[] textures = new Texture2D[iCount];

            iCount = 0;
            for (int i = 0; i < spriteIndexs.Length; i++)
            {
                if (spriteIndexs[i] != -1)
                {
                    textures[iCount] = UIView.GetAView().defaultAtlas[spriteNames[spriteIndexs[i]]].texture;
                    iCount++;
                }
            }

            AddTextures(textures);
        }

        public static void AddTextureSpriteInfo(UITextureAtlas.SpriteInfo spriteInfo)
        {
            if (spriteInfo == null || spriteInfo.name == "" || spriteInfo.texture == null || (m_atlas[spriteInfo.name] != null))
                return;
            m_atlas.AddSprite(spriteInfo);
        }

        public static void AddTextureSpriteInfoFromFile(string spriteName, string file, RectOffset Border)
        {
            Texture2D texture = MUIUtil.loadTextureFromFile(file);
            if (texture == null)
                return;

            UITextureAtlas.SpriteInfo sprite = new UITextureAtlas.SpriteInfo
            {
                name = spriteName,
                texture = texture,
                border = Border               
            };
            AddTextureSpriteInfo(sprite);
        }

        public static void AddTextures(Texture2D[] newTextures, bool locked = false)
        {
            createStaticAtlas();

            Texture2D[] textures = new Texture2D[m_atlas.count + newTextures.Length];

            for (int i = 0; i < m_atlas.count; i++)
            {
                Texture2D texture2D = m_atlas.sprites[i].texture;

                if (locked)
                {
                    // Locked textures workaround
                    RenderTexture renderTexture = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 0);
                    Graphics.Blit(texture2D, renderTexture);

                    RenderTexture active = RenderTexture.active;
                    texture2D = new Texture2D(renderTexture.width, renderTexture.height);
                    RenderTexture.active = renderTexture;
                    texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
                    texture2D.Apply();
                    RenderTexture.active = active;

                    RenderTexture.ReleaseTemporary(renderTexture);
                }

                textures[i] = texture2D;
                textures[i].name = m_atlas.sprites[i].name;
            }

            for (int i = 0; i < newTextures.Length; i++)
                textures[m_atlas.count + i] = newTextures[i];

            Rect[] regions = m_atlas.texture.PackTextures(textures, m_atlas.padding, 4096, false);

            m_atlas.sprites.Clear();

            for (int i = 0; i < textures.Length; i++)
            {
                UITextureAtlas.SpriteInfo spriteInfo = m_atlas[textures[i].name];
                m_atlas.sprites.Add(new UITextureAtlas.SpriteInfo
                {
                    texture = textures[i],
                    name = textures[i].name,
                    border = (spriteInfo != null) ? spriteInfo.border : new RectOffset(),
                    region = regions[i]
                });
            }

            m_atlas.RebuildIndexes();
        }
    }
}
