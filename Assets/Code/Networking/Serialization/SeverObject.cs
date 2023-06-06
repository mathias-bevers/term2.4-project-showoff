namespace saxion_provided
{
    /**
     * Classes that implement the ISerializable interface can (de)serialize themselves
     * into/out of a Packet instance. See the protocol package for an example.
     */
    public abstract class SeverObject
    {
        /**
         * Write all the data for 'this' object into the Packet
         */
        public abstract void Serialize(Packet packet);
        
        /**
         * Read all the data for 'this' object from the Packet
         */
        public abstract void Deserialize(Packet packet);
    }
}
