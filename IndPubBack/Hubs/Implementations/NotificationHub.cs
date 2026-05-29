using IndPubBack.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace IndPubBack.Hubs.Implementations;

[Authorize]
public sealed class NotificationHub : Hub<INotificationClient>;