using System;
using System.Collections.Generic;
using System.Linq;
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

        // GET api/colors/ff0000
        [HttpGet("{id}")]
        public async Task<Color> Get(string id)
        {
            var grain = _client.GetGrain<IColorGrain>(id);
            return await grain.GetColor();
        }

        // POST api/colors/00ff00
        [HttpPost]
        public async Task<ActionResult> Post(string id, [FromBody]ColorTranslation value)
        {
            var grain = _client.GetGrain<IColorGrain>(id);
            await grain.AddTranslation(value);
            return Ok();
        }

    }
}
