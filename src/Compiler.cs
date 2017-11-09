﻿using dotless.Core;
using dotless.Core.Loggers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebOptimizer.Dotless
{
    /// <summary>
    /// Compiles Sass files
    /// </summary>
    /// <seealso cref="WebOptimizer.IProcessor" />
    public class Compiler : IProcessor
    {
        /// <summary>
        /// Gets the custom key that should be used when calculating the memory cache key.
        /// </summary>
        public string CacheKey(HttpContext context) => string.Empty;

        /// <summary>
        /// Executes the processor on the specified configuration.
        /// </summary>
        public Task ExecuteAsync(IAssetContext context)
        {
            var content = new Dictionary<string, byte[]>();
            var env = (IHostingEnvironment)context.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment));
            IFileProvider fileProvider = context.Asset.GetFileProvider(env);

            var engine = new EngineFactory().GetEngine();

            foreach (string route in context.Content.Keys)
            {
                IFileInfo file = fileProvider.GetFileInfo(route);
                                               
                engine.CurrentDirectory = Path.GetDirectoryName(file.PhysicalPath);
                var css = engine.TransformToCss(context.Content[route].AsString(), null);               

                content[route] = System.Text.Encoding.UTF8.GetBytes(css);
            }

            context.Content = content;

            return Task.CompletedTask;
        }
    }   
}
