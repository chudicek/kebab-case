using Microsoft.AspNetCore.Mvc;

namespace Kebabos.Controllers;

public class ErrorController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error() => Problem();
}