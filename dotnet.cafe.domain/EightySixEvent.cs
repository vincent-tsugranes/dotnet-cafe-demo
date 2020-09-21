namespace dotnet.cafe.domain
{
    public class EightySixEvent : Event
    {
        private Item item { get; set; }

        private EventType eventType { get; set; }

        public EightySixEvent()
        {
            this.eventType = EventType.EIGHTY_SIX;
        }

        public EightySixEvent(Item item) {
            this.item = item;
        }

        public EventType getEventType()
        {
            return eventType;
        }
    }
}