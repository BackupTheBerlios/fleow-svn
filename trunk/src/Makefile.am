MCS_FLAGS = -debug -pkg:tao-opengl-glu,tao-devil,gtkglarea-sharp -r:System.Data
ASSEMBLY = Fleow.dll
SOURCES = \
	$(srcdir)/Plugin.cs \
	$(srcdir)/FleowPane.cs \
	$(srcdir)/FleowEngine.cs \
	$(srcdir)/FleowEngineCam.cs \
	$(srcdir)/FleowEngineLights.cs \
	$(srcdir)/FleowEngineCovers.cs \
	$(srcdir)/FleowConfigDialog.cs \
	$(srcdir)/FleowDatabase.cs

plugindir = $(PLUGINDIR)
plugin_SCRIPTS = $(ASSEMBLY) $(ASSEMBLY).mdb

all: $(ASSEMBLY)

$(ASSEMBLY): $(SOURCES)
	$(MCS) $(MCS_FLAGS) -out:$(ASSEMBLY) -target:library $(BANSHEE_LIBS) $(SOURCES)

EXTRA_DIST = $(SOURCES)
CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb 
DISTCLEANFILES = Makefile.in 

