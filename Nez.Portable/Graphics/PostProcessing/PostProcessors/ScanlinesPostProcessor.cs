﻿

namespace Nez
{
	public class ScanlinesPostProcessor : PostProcessor<ScanlinesEffect>
	{
		public ScanlinesPostProcessor( int executionOrder ) : base( executionOrder )
		{
			Effect = new ScanlinesEffect();
		}
	}
}

