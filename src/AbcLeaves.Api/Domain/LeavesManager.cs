﻿using System.Threading.Tasks;
using AbcLeaves.Api.Models;
using AbcLeaves.Api.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AbcLeaves.Api.Domain
{
    public class LeavesManager : ILeavesManager
    {
        private readonly IMapper mapper;
        private readonly ILeavesRepository leavesRepository;
        private readonly IGoogleCalendarManager googleCalendarManager;

        public LeavesManager(
            IMapper mapper,
            ILeavesRepository leavesRepository,
            IGoogleCalendarManager googleCalendarManager)
        {
            this.mapper = mapper;
            this.leavesRepository = leavesRepository;
            this.googleCalendarManager = googleCalendarManager;
        }

        public async Task<LeaveApplyResult> ApplyAsync(LeaveApplyDto leaveDto)
        {
            var leave = mapper.Map<LeaveApplyDto, Leave>(leaveDto);
            await leavesRepository.InsertAsync(leave);
            return LeaveApplyResult.Success(leave);
        }

        public async Task<LeaveApproveResult> ApproveAsync(int leaveId)
        {
            var leave = leavesRepository.GetById(leaveId);
            if (leave == null)
            {
                return LeaveApproveResult.FailNotFound(leaveId);
            }
            if (leave.Status == LeaveStatus.Approved)
            {
                return LeaveApproveResult.Fail($"Leave id={leaveId} has already been approved");
            }
            try
            {
                leave.Status = LeaveStatus.Approved;
                await leavesRepository.UpdateAsync(leave);
            }
            catch (DbUpdateConcurrencyException)
            {
                return LeaveApproveResult.Fail($"Leave id={leaveId} is being updated by another user");
            }

            var eventPublishDto = mapper.Map<Leave, UserEventPublishDto>(leave);
            var shareResult = await googleCalendarManager.PublishUserEventAsync(eventPublishDto);
            return LeaveApproveResult.Success(shareResult);
        }

        public async Task<LeaveDeclineResult> DeclineAsync(int leaveId)
        {
            var leave = leavesRepository.GetById(leaveId);
            if (leave == null)
            {
                return LeaveDeclineResult.FailNotFound(leaveId);
            }
            if (leave.Status == LeaveStatus.Approved)
            {
                return LeaveDeclineResult.Fail(
                    $"Cannot decline leave id={leaveId} " +
                    "that has already been approved"
                );
            }
            if (leave.Status == LeaveStatus.Declined)
            {
                return LeaveDeclineResult.Fail($"Leave id={leaveId} has already been declined");
            }
            try
            {
                leave.Status = LeaveStatus.Declined;
                await leavesRepository.UpdateAsync(leave);
            }
            catch (DbUpdateConcurrencyException)
            {
                return LeaveDeclineResult.Fail($"Leave id={leaveId} is being updated by another user");
            }
            return LeaveDeclineResult.Success;
        }
    }
}