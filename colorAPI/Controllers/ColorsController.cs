using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using GrainInterfaces;
using GrainInterfaces.States;

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
            try
            {
                var grain = _client.GetGrain<IColorGrain>(rgb);
                var resultat = await grain.GetColor();
                return Ok(resultat);
            }
            catch (ColorsException e)
            {
                return BadRequest(new { Message = e.Message });
            }
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
        /// <response code="400">Unable to delete translation</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var rgb = id.ToUpper();
            try
            {
                var grain = _client.GetGrain<IColorGrain>(rgb);
                await grain.DeleteColor();
                return Ok(new { Message = "All RGB translations deleted" });
            }
            catch (ColorsException e)
            {
                return BadRequest(new { Message = e.Message });
            }
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
        /// <response code="400">Error adding translation</response>
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, [FromBody]ColorTranslation value)
        {
            var rgb = id.ToUpper();
            try
            {
                var grain = _client.GetGrain<IColorGrain>(rgb);
                await grain.AddTranslation(value);
                return Ok(new { Message = "Translation added" });
            }
            catch (ColorsException e)
            {
                return BadRequest(new { Message = e.Message });
            }
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
        /// <response code="400">Unable to modify translation</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Modify(string id, [FromBody]ColorTranslation value)
        {
            var rgb = id.ToUpper();
            try
            {
                var grain = _client.GetGrain<IColorGrain>(rgb);
                await grain.ModifyTranslation(value);
                return Ok(new { Message = "Translation modified" });

            }
            catch (ColorsException e)
            {
                return BadRequest(new { Message = e.Message });
            }
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
        /// <response code="400">Unable to delete</response>
        [HttpDelete("{id}/{lang}")]
        public async Task<IActionResult> Delete(string id, string lang)
        {
            var rgb = id.ToUpper();
            try
            {
                var grain = _client.GetGrain<IColorGrain>(rgb);
                await grain.DeleteTranslation(lang);
                return Ok(new { Message = "Translation deleted" });
            }
            catch (ColorsException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

    }
}
