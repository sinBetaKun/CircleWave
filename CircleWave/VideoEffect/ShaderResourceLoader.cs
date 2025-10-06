﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CircleWave.VideoEffect
{
    internal class ShaderResourceLoader
    {
        public static byte[] GetShaderResource(string name)
        {
            //埋め込まれたシェーダーを読み込む
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"CircleWave.Shaders.{name}";
            using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new Exception($"Resource {resourceName} not found.");
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
