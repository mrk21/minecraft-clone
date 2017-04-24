namespace MinecraftClone.Infrastructure {
	interface IEntity<TId> {
		TId Id {
			get;
		}
	}
}