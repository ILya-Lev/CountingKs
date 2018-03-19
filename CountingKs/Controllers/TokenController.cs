using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class TokenController : BaseApiController
    {
        public TokenController(ICountingKsRepository repo, IModelFactory modelFactory)
            : base(repo, modelFactory)
        {
        }

        public HttpResponseMessage Post([FromBody] TokenRequestModel model)
        {
            try
            {
                var developer = Repository.GetApiUsers().FirstOrDefault(u => u.AppId == model.ApiKey);
                if (developer != null)
                {
                    var secret = developer.Secret;
                    var key = Convert.FromBase64String(secret);
                    var provider = new System.Security.Cryptography.HMACSHA256(key);
                    var hash = provider.ComputeHash(Encoding.UTF8.GetBytes(developer.AppId));
                    var signature = Convert.ToBase64String(hash);

                    if (model.Signature == signature)
                    {
                        var rawTokenInfo = $"{developer.AppId} {DateTime.UtcNow:d}";
                        var rawTokenByte = Encoding.UTF8.GetBytes(rawTokenInfo);
                        var token = provider.ComputeHash(rawTokenByte);

                        var authToken = new AuthToken
                        {
                            Token = Convert.ToBase64String(token),
                            Expiration = DateTime.UtcNow.AddDays(7),
                            ApiUser = developer
                        };

                        if (Repository.Insert(authToken) && Repository.SaveAll())
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, ModelFactory.Create(authToken));
                        }
                    }
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception xc)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, xc);
            }
        }
    }
}
