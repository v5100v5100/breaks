#include "pch.h"

namespace M6502Core
{
	M6502::M6502()
	{
		decoder = new Decoder;
	}

	M6502::~M6502()
	{
		delete decoder;
	}

	void M6502::sim(BaseLogic::TriState inputs[], BaseLogic::TriState outputs[], BaseLogic::TriState inOuts[])
	{
		BaseLogic::TriState decoder_inputs[Decoder::inputs_count];
		BaseLogic::TriState decoder_outputs[Decoder::outputs_count];

		decoder->sim(decoder_inputs, decoder_outputs);
	}

}
