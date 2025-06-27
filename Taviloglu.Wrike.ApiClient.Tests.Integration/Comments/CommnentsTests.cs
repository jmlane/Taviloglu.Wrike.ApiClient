﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Taviloglu.Wrike.Core.Comments;

namespace Taviloglu.Wrike.ApiClient.Tests.Integration.Comments
{
    [TestFixture, Order(4)]
    public class CommnentsTests
    {
        const string DefaultCommentId = "IEACGXLUIMHLQB2D";
        const string DefaultTaskId = "IEACGXLUKQO6DCNW";
        const string DefaultFolderId = "IEACGXLUI4O6C46Q";

        private string _addedCommentId = string.Empty;

        [OneTimeTearDown]
        public void ReturnToDefaults()
        {
            var comments = WrikeClientFactory.GetWrikeClient().Comments.GetAsync().Result;

            foreach (var comment in comments)
            {
                if (comment.Id != DefaultCommentId)
                {
                    WrikeClientFactory.GetWrikeClient().Comments.DeleteAsync(comment.Id).Wait();
                }
            }
        }

        [Test, Order(1)]
        public void CreateAsync_ShouldAddNewNewCommentToDefaultTask()
        {
            var newComment = new WrikeTaskComment("My new test comment", DefaultTaskId);
            
            var createdComment = WrikeClientFactory.GetWrikeClient().Comments.CreateAsync(newComment, plainText: true).Result;

            _addedCommentId = createdComment.Id; //To be used in GetAsyncWithIds_ShouldReturnDefaultComment

            Assert.IsNotNull(createdComment);
            Assert.AreEqual(newComment.Text, createdComment.Text);
            Assert.AreEqual(newComment.TaskId, createdComment.TaskId);
        }

        [Test, Order(2)]
        public void CreateAsync_ShouldAddNewNewCommentToDefaultFolder()
        {
            var newComment = new WrikeFolderComment("My new test comment", DefaultFolderId);

            var createdComment = WrikeClientFactory.GetWrikeClient().Comments.CreateAsync(newComment, plainText: true).Result;

            Assert.IsNotNull(createdComment);
            Assert.AreEqual(newComment.Text, createdComment.Text);
            Assert.AreEqual(newComment.FolderId, createdComment.FolderId);
        }

        [Test, Order(3)]
        public void UpdateAsync_ShouldUpdateCommentText()
        {
            var newComment = new WrikeTaskComment("My new test comment", DefaultTaskId);

            newComment = WrikeClientFactory.GetWrikeClient().Comments.CreateAsync(newComment, plainText: true).Result;

            var expectedCommentText = "My new test comment [Updated]";
            var updatedComment = WrikeClientFactory.GetWrikeClient().Comments.UpdateAsync(newComment.Id, expectedCommentText, plainText: true).Result;

            Assert.IsNotNull(updatedComment);
            Assert.AreEqual(expectedCommentText, updatedComment.Text);
        }

        [Test, Order(4)]
        public void GetAsync_ShouldReturnComments()
        {
            var comments = WrikeClientFactory.GetWrikeClient().Comments.GetAsync().Result;
            Assert.IsNotNull(comments);
            Assert.Greater(comments.Count, 0);
        }


        

        [Test, Order(5)]
        public void GetInTaskAsync_ShouldReturnComments()
        {
            var newComment = new WrikeTaskComment("My new test comment", DefaultTaskId);
            var createdComment = WrikeClientFactory.GetWrikeClient().Comments.CreateAsync(newComment, plainText: true).Result;

            var comments = WrikeClientFactory.GetWrikeClient().Comments.GetInTaskAsync(DefaultTaskId).Result;

            Assert.IsNotNull(comments);
            Assert.Greater(comments.Count, 0);
            Assert.IsTrue(comments.Any(c => c.Id == createdComment.Id));
        }

        [Test, Order(6)]
        public void GetInFolderAsync_ShouldReturnComments()
        {
            var newComment = new WrikeFolderComment("My new test comment", DefaultFolderId);
            var createdComment = WrikeClientFactory.GetWrikeClient().Comments.CreateAsync(newComment, plainText: true).Result;

            var comments = WrikeClientFactory.GetWrikeClient().Comments.GetInFolderAsync(DefaultFolderId).Result;

            Assert.IsNotNull(comments);
            Assert.Greater(comments.Count, 0);
            Assert.IsTrue(comments.Any(c => c.Id == createdComment.Id));
        }

        [Test, Order(7)]
        public void GetAsyncWithIds_ShouldReturnDefaultComment()
        {
            var comments = WrikeClientFactory.GetWrikeClient().Comments.GetAsync(new List<string> { _addedCommentId }).Result;
            Assert.IsNotNull(comments);
            Assert.AreEqual(1, comments.Count);
            Assert.AreEqual(_addedCommentId, comments.First().Id);
        }

        

        

        [Test, Order(8)]
        public void DeleteAsync_ShouldDeleteComment()
        {
            var newComment = new WrikeTaskComment("My new test comment", DefaultTaskId);
            newComment = WrikeClientFactory.GetWrikeClient().Comments.CreateAsync(newComment, plainText: true).Result;

            WrikeClientFactory.GetWrikeClient().Comments.DeleteAsync(newComment.Id).Wait();

            var comments = WrikeClientFactory.GetWrikeClient().Comments.GetAsync().Result;
            var isCommentDeleted = !comments.Any(c => c.Id == newComment.Id);

            Assert.IsTrue(isCommentDeleted);
        }
    }
}
