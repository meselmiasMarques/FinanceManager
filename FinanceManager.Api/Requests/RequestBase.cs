using System.Text.Json.Serialization;

namespace FinanceManager.Requests
{
    public abstract class RequestBase
    {
        /// <summary>
        /// Inquilino da requisição. Preenchido <b>exclusivamente</b> no servidor a partir do
        /// token (ver <c>IUserContext</c>); <see cref="JsonIgnoreAttribute"/> impede que o
        /// cliente forje o dono do registro pelo corpo do JSON (DC-02).
        /// </summary>
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
