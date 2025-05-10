using feishu_doc_export.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace feishu_doc_export
{
    public static class DocumentPathGenerator
    {
        /// <summary>
        /// 文档objToken和路径的映射
        /// </summary>
        private static Dictionary<string, string> documentPaths;
        /// <summary>
        /// 文档nodeToken和路径的映射
        /// </summary>
        private static Dictionary<string, string> documentPaths2;

        public static void GenerateDocumentPaths(List<WikiNodeItemDto> documents, string rootFolderPath)
        {
            documentPaths = new Dictionary<string, string>();
            documentPaths2 = new Dictionary<string, string>();

            var topDocument = documents.Where(x => string.IsNullOrWhiteSpace(x.ParentNodeToken));
            foreach (var document in topDocument)
            {
                GenerateDocumentPath(document, rootFolderPath, documents);
            }

        }

        private static void GenerateDocumentPath(WikiNodeItemDto document, string parentFolderPath, List<WikiNodeItemDto> documents)
        {
            // 处理文件名，只保留"-"之前的数字部分
            string processedTitle = ProcessFileName(document.Title);
            
            // 替换文件名中的非法字符
            string title = Regex.Replace(processedTitle, @"[\\/:\*\?""<>\|]", "-");
            string documentFolderPath = Path.Combine(parentFolderPath, title);

            documentPaths[document.ObjToken] = documentFolderPath;
            documentPaths2[document.NodeToken] = documentFolderPath;

            foreach (var childDocument in GetChildDocuments(document, documents))
            {
                GenerateDocumentPath(childDocument, documentFolderPath, documents);
            }
        }

        /// <summary>
        /// 处理文件名，只保留"-"之前的数字部分
        /// </summary>
        /// <param name="fileName">原始文件名</param>
        /// <returns>处理后的文件名</returns>
        private static string ProcessFileName(string fileName)
        {
            // 使用正则表达式匹配"-"之前的数字部分
            var match = Regex.Match(fileName, @"^(\d+)-");
            if (match.Success)
            {
                // 如果匹配到数字前缀，返回数字部分
                return match.Groups[1].Value;
            }
            // 如果没有匹配到数字前缀，返回原始文件名
            return fileName;
        }

        private static IEnumerable<WikiNodeItemDto> GetChildDocuments(WikiNodeItemDto document, List<WikiNodeItemDto> documents)
        {
            return documents.Where(d => d.ParentNodeToken == document.NodeToken);
        }

        /// <summary>
        /// 获取文档的存储路径
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        public static string GetDocumentPath(string objToken)
        {
            documentPaths.TryGetValue(objToken, out string path);
            return path;
        }

        /// <summary>
        /// 获取文档的存储路径
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        public static string GetDocumentPathByNodeToken(string nodeToken)
        {
            documentPaths2.TryGetValue(nodeToken, out string path);
            return path;
        }
    }
}
