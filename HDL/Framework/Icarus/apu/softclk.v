// A unit test for checking SoftCLK in different modes.

// This is not a unit test right now, we are just doing some preliminary SoftCLK fiddling and looking at the signals.

`timescale 1ns/1ns

module SoftCLK_Run();

	reg CLK;
	reg n_RES;
	reg n_IRQ;
	reg n_NMI;
	wire [15:0] AddrPads;
	wire [7:0] DataPads;
	wire M2_Out;
	wire RnW_fromapu;
	wire [1:0] nIN_Ports;
	wire [2:0] OUT_Ports;

	// Tune CLK/ACLK timing according to 2A03
	always #23.28 CLK = ~CLK;

	APU apu (
		.n_RES(n_RES),
		.A(AddrPads),
		.D(DataPads),
		.CLK(CLK),
		.DBG(1'b0),
		.M2(M2_Out),
		.n_IRQ(n_IRQ),
		.n_NMI(n_NMI),
		.RnW(RnW_fromapu),
		.n_IN0(nIN_Ports[0]),
		.n_IN1(nIN_Ports[1]),
		.OUT0(OUT_Ports[0]),
		.OUT1(OUT_Ports[1]),
		.OUT2(OUT_Ports[2]) );	

	initial begin

		$display("Test SoftCLK");

		CLK <= 1'b0;
		n_RES <= 1'b1;
		n_IRQ <= 1'b1;
		n_NMI <= 1'b1;

		$dumpfile("softclk.vcd");
		$dumpvars(0, apu);

		// Reset cycle

		n_RES <= 1'b0;
		repeat (1) @ (posedge CLK);
		n_RES <= 1'b1;

		// Set mode (by hand)
		// Set the value of the register $4017[7] bit directly  (0 - normal mode, 1 - extended mode)

		apu.frame_cnt.fcnt_ctl.mode.val <= 1'b0;

		// Set timer interrupt enable $4017[6] (0 - enable)

		apu.frame_cnt.fcnt_ctl.mask.val <= 1'b0;

		// Taking into account the divider 2A03 (by 6) 30k iterations should be enough for any SoftCLK mode.
		// Repeat batch 4 times

		repeat (30000*6*4) @ (posedge CLK);
		//repeat (1024) @ (posedge CLK);
		$finish(0);
	end

endmodule // SoftCLK_Run
