####################################################
###### Copyright(c) GHI Electronics, LLC ######
####################################################

OUTFILE=SensorPoller
OUTDIR=../Resources
FILENAME=SensorPoller
LINKERSCRIPT = RLP_LinkScript.lds


INCL=./include

CC		=arm-none-eabi-gcc
LD		=arm-none-eabi-gcc

CCFLAGS=  -g -mlittle-endian -mcpu=arm7tdmi  -Wall -I. -I$(INCL)
CCFLAGS+= -mapcs-frame -fno-builtin -g0

LDFLAGS =-nostartfiles -Wl,--Map -Wl,$(OUTDIR)/$(OUTFILE).map
LDFLAGS+=-lc -lgcc -Wl,--omagic
LDFLAGS+=-T $(LINKERSCRIPT)

OBJS+= $(FILENAME).o

rebuild: clean all del_o

all: $(OBJS)
	$(LD) $(LDFLAGS) -o $(OUTDIR)/$(OUTFILE).elf $(OBJS)
	

$(FILENAME).o: $(FILENAME).c 
	$(CC) -c $(CCFLAGS) $(FILENAME).c -o $(FILENAME).o

clean:
	-rm *.o $(OUTDIR)/*.elf $(OUTDIR)/*.map

del_o:
	-rm *.o

del_map:
	-rm $(OUTDIR)/*.map