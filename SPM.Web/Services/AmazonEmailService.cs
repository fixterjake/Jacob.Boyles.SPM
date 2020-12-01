using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace SPM.Web.Services
{
    public class AmazonEmailService
    {
        /// <summary>
        ///     Function to send an email using Amazon's simple email service
        ///     This version will only send to one single user
        /// </summary>
        /// <param name="sender">Email that will send the email</param>
        /// <param name="to">Who to send it to</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email HTML body</param>
        /// <param name="textBody">Email text body</param>
        /// <returns>Email send response object</returns>
        public static async Task<SendEmailResponse> SendEmail(string sender, string to, string subject, string body,
            string textBody)
        {
            // Create an Amazon SES client
            using var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1);

            // Setup all fields for the email request
            var sendRequest = new SendEmailRequest
            {
                Source = sender,
                Destination = new Destination
                {
                    ToAddresses = new List<string> {to}
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = body
                        },
                        Text = new Content
                        {
                            Charset = "UTF-8",
                            Data = textBody
                        }
                    }
                }
            };
            try
            {
                // Send the email async
                var response = await client.SendEmailAsync(sendRequest);

                // Return the response
                return response;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine(ex.Message);

                // Return null to indicate bad request
                return null;
            }
        }

        /// <summary>
        ///     Function to send an email using Amazon's simple email service
        ///     This version will send to many users
        /// </summary>
        /// <param name="sender">Email that will send the email</param>
        /// <param name="to">Who to send it to</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email HTML body</param>
        /// <param name="textBody">Email text body</param>
        /// <returns>Email send response object</returns>
        public static async Task<SendEmailResponse> SendEmail(string sender, List<string> to, string subject,
            string body, string textBody)
        {
            // Create an Amazon SES client
            using var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1);

            // Setup all fields for the email request
            var sendRequest = new SendEmailRequest
            {
                Source = sender,
                Destination = new Destination
                {
                    BccAddresses = to
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = body
                        },
                        Text = new Content
                        {
                            Charset = "UTF-8",
                            Data = textBody
                        }
                    }
                }
            };
            try
            {
                // Send the email async
                var response = await client.SendEmailAsync(sendRequest);

                // Return the response
                return response;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine(ex.Message);

                // Return null to indicate bad request
                return null;
            }
        }
    }
}