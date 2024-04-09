using Infrastructure.DTOs.Movie;
using Infrastructure.Services.EmailService;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;

namespace Infrastructure.Services.Email;
public class EmailService : IEmailService
{

    public readonly EmailSettings EmailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        EmailSettings = emailSettings.Value;
    }
    /// <summary>
    /// Share the movie via email
    /// </summary>
    /// <param name="email">The email where the movie is to be shared.</param>
    /// <param name="movie">The movie that needs to be shared.</param>
    /// <returns></returns>
    public async Task ShareMovie(string emailAddress, MovieDetailDTO movie)
    {
        var bodyBuilder = new BodyBuilder();
        //bodyBuilder.HtmlBody = $"""
        //    <html>
        //        <body>
        //            You have been shared a movie!
        //            <img class="movie-image" src="data:image/jpeg;base64,{Convert.ToBase64String(movie.Image)}" alt="Movie Poster">

        //            The movie is titled: {movie.Name}.
        //            Synopsis: {movie.Description}
        //            Avg Rating: {(movie.TotalRates == 0 ? 0 : movie.Rating / (float)movie.TotalRates)}
        //        <body>
        //    <html>
        //    """;

        // Load the image bytes into a MemoryStream
        var imageStream = new MemoryStream(movie.Image);

        // Create an attachment with Content-ID
        var imageAttachment = new MimePart("image", "jpeg")
        {
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = "movie.jpg"
        };
        imageAttachment.ContentId = MimeUtils.GenerateMessageId();
        imageAttachment.Content = new MimeContent(imageStream);

        // Add the attachment to the body builder
        bodyBuilder.Attachments.Add(imageAttachment);

        // Embed the image in the HTML body using CID
        bodyBuilder.HtmlBody = $@"
    <html>
        <body>
            <h3>You have been shared a movie!</h3>
            <img class='movie-image' src='cid:{imageAttachment.ContentId}' alt='Movie Poster' width='700' height='300' >

            <p> The movie is titled: <strong> {movie.Name} </strong>. </p>
            <p><strong>Synopsis: <strong> {movie.Description} </p>
            <p><strong>Avg Rating: </strong> {(movie.TotalRates == 0 ? 0 : movie.Rating / (float)movie.TotalRates)} </p>
        </body>
    </html>
";


        var email = new MimeMessage();
        email.Body = bodyBuilder.ToMessageBody();

        email.From.Add(MailboxAddress.Parse(EmailSettings.From));
        email.Sender = new MailboxAddress(EmailSettings.Username, EmailSettings.From);
        email.To.Add(MailboxAddress.Parse(emailAddress));

        email.Subject = "Shared a movie";

        using var smtp = new SmtpClient();

        smtp.AuthenticationMechanisms.Remove("XOAUTH2");
        await smtp.ConnectAsync(EmailSettings.SmtpServer, EmailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(EmailSettings.From, EmailSettings.Password);
        await smtp.SendAsync(email);
        smtp.Dispose();
    }
}
