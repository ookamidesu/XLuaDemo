---@class EventDispatcher : BaseObject
---@field public eventSys EventSystem
---事件派发器.
local EventDispatcher = BaseClass()

function EventDispatcher:Constructor()
    ---@type EventSystem
    self.eventSys = EventSystem.New()
end


function EventDispatcher:Bind(eventId, func, owner)
    return self.eventSys:Bind(eventId, func,owner)
end
function EventDispatcher:Unbind(bindId)
    self.eventSys:Unbind(bindId)
end
function EventDispatcher:UnbindAll()
    self.eventSys:UnbindAll()
end

function EventDispatcher:Fire(eventId, ...)
    self.eventSys:Fire(eventId, ...)
end

--[[function EventDispatcher:DelayFire(eventId, ...)
    self.eventSys:DelayFire(eventId, ...)
end]]

return EventDispatcher
