; A test to check all opcodes, and at the same time check Breakasm.

org $200
processor 6502

Reset:

; Branches / Jump / JSR

	bpl BplLabel
BplLabel:
	bmi BmiLabel
BmiLabel:
	bvc BvcLabel
BvcLabel:
	bvs BvsLabel
BvsLabel:
	bcc BccLabel
BccLabel:
	bcs BcsLabel
BcsLabel:
	bne BneLabel
BneLabel:
	beq BeqLabel
BeqLabel:

	jsr Sub1
	jmp JumpOverSub
Sub1:
	rts
JumpOverSub:

	jmp (IndirectJumpAddr)
IndirectLabel:

; TwoCycle opcodes

	ldy #1
	cpy #1
	cpx #1
	ldx #1

	clc
	sec
	cli
	sei
	clv
	sed 			; First set the D flag, then reset it. We don't want to use BCD yet.
	cld

	dey
	tya
	tay
	iny
	inx

	ora #1
	and #1
	eor #1
	adc #1
	lda #1
	cmp #1
	sbc #1

	asl a
	rol a
	lsr a
	ror a

	txa
	tax	
	txs
	tsx
	dex
	nop

; ind

	ora x, $1
	ora $1, y
	and x, $1
	and $1, y
	eor x, $1
	eor $1, y
	adc x, $1
	adc $1, y
	sta x, $1
	sta $1, y
	lda x, $1
	lda $1, y
	cmp x, $1
	cmp $1, y
	sbc x, $1
	sbc $1, y

; zpg

	bit $1
	sty $1
	sty $1, x
	ldy $1
	ldy $1, x
	cpy $1
	cpx $1

	ora $1
	ora $1, x
	and $1
	and $1, x
	eor $1
	eor $1, x
	adc $1
	adc $1, x
	sta $1
	sta $1, x
	lda $1
	lda $1, x
	cmp $1
	cmp $1, x
	sbc $1
	sbc $1, x

	asl $1
	asl $1, x
	rol $1
	rol $1, x
	lsr $1
	lsr $1, x
	ror $1
	ror $1, x
	stx $1
	stx $1, y
	ldx $1
	ldx $1, y
	dec $1
	dec $1, x
	inc $1
	inc $1, x

; abs, Y

	ora $4000, y
	and $4000, y
	eor $4000, y
	adc $4000, y
	sta $4000, y
	lda $4000, y
	cmp $4000, y
	sbc $4000, y

; abs

	ora $4000
	asl $4000
	bit $4000
	and $4000
	rol $4000
	eor $4000
	lsr $4000
	adc $4000
	ror $4000
	sty $4000
	sta $4000
	stx $4000
	ldy $4000
	lda $4000
	ldx $4000
	cpy $4000
	cmp $4000
	dec $4000
	cpx $4000
	sbc $4000
	inc $4000

; abs, X (except LDX)

	ora $4000, x
	asl $4000, x
	and $4000, x
	rol $4000, x
	eor $4000, x
	lsr $4000, x
	adc $4000, x
	ror $4000, x
	sta $4000, x
	ldy $4000, x
	lda $4000, x
	ldx $4000, Y   ; Care!
	cmp $4000, x
	dec $4000, x
	sbc $4000, x
	inc $4000, x

; pp

	php
	plp
	pha
	pla

; Check BRK and go into an infinite loop.

	brk
Halt:
	jmp Halt

IndirectJumpAddr:
	word IndirectLabel

BogusNMI:
	inx
	rti

BogusIRQBrk:
	iny
	rti

org $fffa
word BogusNMI
word Reset
word BogusIRQBrk