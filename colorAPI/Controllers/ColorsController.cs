using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using GrainInterfaces;
using GrainInterfaces.States;
using System.Text.RegularExpressions;

namespace apicolors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        IClusterClient _client;

        public ColorsController(IClusterClient client)
        {
            _client = client;
        }

        // GET api/colors/ff0000
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var rgb = id.ToUpper();
            if (isRGBCorrect(rgb))
            {
                var grain = _client.GetGrain<IColorGrain>(rgb);
                var resultat = await grain.GetColor();
                return Ok(resultat);
            }
            return BadRequest(new { Message = "Incorrect RGB Code" });
        }

        // POST api/colors/00ff00
        [HttpPost]
        public async Task<IActionResult> Post(string id, [FromBody]ColorTranslation value)
        {
            var rgb = id.ToUpper();
            if (isRGBCorrect(rgb))
            {
                var grain = _client.GetGrain<IColorGrain>(rgb);
                await grain.AddTranslation(value);
                return Ok(new { Message = "Translation added" });
            }
            return BadRequest(new { Message = "Incorrect RGB Code" });
        }

        private bool isRGBCorrect(string value)
        {
            Match result = Regex.Match(value, @"^[0-9A-F]{6}$");
            return result.Success;
        }

    }
}
