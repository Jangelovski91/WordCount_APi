using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordCountController : ControllerBase
    {
        private static readonly string[] Separators = new string[] { " " };

        // GET: api/WordCount
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult PostFile(IFormFile file, CancellationToken token)
        {
            if (file != null)
            {
                var fileContent = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding("iso-8859-1"));

                List<string> words = new List<string>();

                string line;
                while ((line = fileContent.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        
                        string[] splittedLine = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                        words.AddRange(splittedLine);
                    }
                }

                Dictionary<string, int> wordCountRepeat = new Dictionary<string, int>(capacity: words.Count);

                words.ForEach(word =>
                {
                    //for simulating cacelation option
                    Thread.Sleep(50);  
                    
                    if (token.IsCancellationRequested)
                    {
                       return;
                    }
                    string trimmedWord = word.Trim();

                    if (wordCountRepeat.ContainsKey(trimmedWord))
                    {
                        int value = wordCountRepeat[trimmedWord];
                        wordCountRepeat[trimmedWord] = value + 1;
                    }
                    else
                    {
                        wordCountRepeat.Add(trimmedWord, 1);
                    }
                });

                List<WordCountDto> result = new List<WordCountDto>();

                foreach (KeyValuePair<string, int> word in wordCountRepeat.OrderByDescending(x => x.Value))
                {
                    result.Add(new WordCountDto
                    {
                        Word = word.Key,
                        Count = word.Value
                    });
                }

                return Ok(result);
            }
            else
            {
                return BadRequest("No file uploaded!");
            }

        }
    }
}
