---@class EventSystem : BaseObject
---事件派发器.
EventSystem = BaseClass()
EventSystem.allEventCount = 0

function EventSystem:Constructor()
    ---@type Dictionary | Dictionary[] | function[][]
    self.allEvents = Dictionary.New()

    ---@type Dictionary | string[]
    self.allEventIds = Dictionary.New()
end

function EventSystem:BindEvent(eventId,eventFunc,owner)
    if owner then
        local funcTmp = eventFunc
        eventFunc = function ( ... )
            --闭包延长owner的生命周期
            funcTmp(owner, ...)
        end
    end
    EventSystem.allEventCount = EventSystem.allEventCount+1;
    if self.allEvents:ContainsKey(eventId) then
        self.allEvents[eventId]:Add(EventSystem.allEventCount,eventFunc)
    else
        self.allEvents[eventId] = Dictionary.New({[EventSystem.allEventCount] = eventFunc})
    end
    self.allEventIds:Add(EventSystem.allEventCount,eventId)
    return EventSystem.allEventCount
    
end

function EventSystem:Unbind(bindId)
    local eventId = self.allEventIds[bindId]
    self.allEvents[eventId]:Remove(bindId)
end

function EventSystem:UnbindAll()
    self.allEventIds:Clear()
    self.allEvents:Clear()
end

function EventSystem:Fire(eventId,...)
    for bindId, callBack in pairs(self.allEvents[eventId]) do
        callBack(...)
    end
   
end