MCS_FLAGS = -debug -r:/usr/lib/mono/gtkglarea-sharp/gtkglarea-sharp.dll -pkg:tao-opengl-glu -r:/usr/lib/mono/tao/Tao.DevIl.dll -r:System.Data
ASSEMBLY = Fleow.dll
SOURCES = \
	$(srcdir)/Plugin.cs \
	$(srcdir)/FleowPane.cs \
	$(srcdir)/FleowEngine.cs \
	$(srcdir)/FleowDatabase.cs

plugindir = $(PLUGINDIR)
plugin_SCRIPTS = $(ASSEMBLY) $(ASSEMBLY).mdb

all: $(ASSEMBLY)

$(ASSEMBLY): $(SOURCES)
	$(MCS) $(MCS_FLAGS) -out:$(ASSEMBLY) -target:library $(BANSHEE_LIBS) $(SOURCES)

EXTRA_DIST = $(SOURCES)
CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb 
DISTCLEANFILES = Makefile.in 

