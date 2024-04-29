﻿using JWTAuthServer.SharedLibrary.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthServer.API.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        public IActionResult ActionResultInstance<T>(ResponseDto<T> response) where T : class
        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
