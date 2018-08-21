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


        /// <summary>
        /// Get translations from a RGB Code
        /// </summary>
        /// <remarks>
        /// example:
        ///
        ///     GET /api/color/ff0000
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <response code="200">Returns the translation</response>
        /// <response code="400">The RGB Code is incorrect</response>
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

        /// <summary>
        /// Delete all translations from RGB Code
        /// </summary>
        /// <remarks>
        /// example:
        ///
        ///     DELETE /api/color/ff0000
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <response code="200">Translations deleted</response>
        /// <response code="404">RGB code not found</response>
        /// <response code="400">The RGB Code is incorrect</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var rgb = id.ToUpper();
            if (isRGBCorrect(rgb))
            {
                var grain = _client.GetGrain<IColorGrain>(rgb);
                var resultat = await grain.DeleteColor();
                if (!resultat)
                {
                    return NotFound(new { Message = "RGB Not found" });
                }
                return Ok(new { Message = "All RGB translations deleted" });
            }
            return BadRequest(new { Message = "Incorrect RGB Code" });
        }


        /// <summary>
        /// Add a translation to a RGB color
        /// </summary>
        /// <remarks>
        /// example:
        ///
        ///     POST /api/color/ff0000
        ///     {
        ///         "Language":"catalan",
        ///         "Name": "vermell"
        ///     }
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <response code="200">Translation added</response>
        /// <response code="400">The RGB Code is incorrect</response>
        [HttpPost("{id}")]
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

        /// <summary>
        /// Modify translation from color
        /// </summary>
        /// <remarks>
        /// example:
        ///
        ///     PUT /api/color/ff0000
        ///     {
        ///         "Language":"catalan",
        ///         "Name": "vermell"
        ///     }
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <response code="200">Translation changed</response>
        /// <response code="404">Translation not found</response>
        /// <response code="400">The RGB Code is incorrect</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Modify(string id, [FromBody]ColorTranslation value)
        {
            var rgb = id.ToUpper();
            if (isRGBCorrect(rgb))
            {
                var grain = _client.GetGrain<IColorGrain>(rgb);
                var result = await grain.ModifyTranslation(value);
                if (result == false)
                {
                    return NotFound(new { Message = "Translation not found" });
                }
                return Ok(new { Message = "Translation modified" });
            }
            return BadRequest(new { Message = "Incorrect RGB Code" });
        }

        /// <summary>
        /// Modify translation from color
        /// </summary>
        /// <remarks>
        /// example:
        ///
        ///     DELETE /api/color/ff0000/catalan
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <response code="200">Translation removed</response>
        /// <response code="404">Translation not found</response>
        /// <response code="400">The RGB Code is incorrect</response>
        [HttpDelete("{id}/{lang}")]
        public async Task<IActionResult> Delete(string id, string lang)
        {
            var rgb = id.ToUpper();
            if (isRGBCorrect(rgb))
            {
                var grain = _client.GetGrain<IColorGrain>(rgb);
                var result = await grain.DeleteTranslation(lang);
                if (result == false)
                {
                    return NotFound(new { Message = "Translation not found" });
                }
                return Ok(new { Message = "Translation deleted" });
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
