﻿using Microsoft.EntityFrameworkCore;
using ReportApp.Shared;

namespace ReportApp.API.Models
{
    public class FeedbackModel:IFeedbackModel
    {
        private readonly AppDbContext _appDbContext;

        public FeedbackModel(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Feedback> AddFeedback(Feedback feedback)
        {
            var result = await _appDbContext.Feedbacks.AddAsync(feedback);
            await _appDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public IEnumerable<FeedbackWithUserDetails> GetAllFeedbacks()
        {
            var feedbacksWithUsers = _appDbContext.Feedbacks
         .Include(f => f.User) // Certifique-se de incluir o usuário
         .Select(f => new FeedbackWithUserDetails
         {
             FeedbackId = f.FeedbackId,
             UserId = f.UserId,
             Timestamp = f.Timestamp,
             Ranking = f.Ranking,
             Comments = f.Comments,
             UserName = f.User.UserName,
             UserEmail = f.User.Email
         })
         .ToList();

            return feedbacksWithUsers;
        }

        public FeedbackWithUserDetails GetFeedbackById(int feedbackId)
        {
            var feedbackWithUser = _appDbContext.Feedbacks
       .Include(f => f.User) // Certifique-se de incluir o usuário
       .Where(f => f.FeedbackId == feedbackId)
       .Select(f => new FeedbackWithUserDetails
       {
           FeedbackId = f.FeedbackId,
           UserId = f.UserId,
           Timestamp = f.Timestamp,
           Ranking = f.Ranking,
           Comments = f.Comments,
           UserName = f.User.UserName,
           UserEmail = f.User.Email
       })
       .FirstOrDefault();

            return feedbackWithUser;
        }
    }
}