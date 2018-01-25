using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight.CommandWpf;
using GestionFormation.App.Core;
using GestionFormation.Applications.Conventions;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Contacts.Queries;
using GestionFormation.CoreDomain.Conventions;
using GestionFormation.CoreDomain.Places.Queries;
using GestionFormation.CoreDomain.Sessions.Queries;

namespace GestionFormation.App.Views.Places
{
    public class GestionConventionWindowVm : PopupWindowVm
    {
        private readonly Guid _conventionId;
        private readonly IApplicationService _applicationService;
        private readonly ICompleteSessionResult _sessionResult;
        private readonly IPlacesQueries _placesQueries;
        private readonly IContactQueries _contactQueries;
        private readonly IDocumentRepository _documentRepository;
        private readonly TypeConvention _typeConvention;
        private readonly string _numeroConvention;
        private string _nom;
        private string _prenom;
        private string _email;
        private string _telephone;
        private ObservableCollection<IConventionPlaceResult> _places;
        private string _documentPath;

        public GestionConventionWindowVm(Guid conventionId, ICompleteSessionResult sessionResult, TypeConvention typeConvention, string numeroConvention, IPlacesQueries placesQueries, IApplicationService applicationService, IContactQueries contactQueries, IDocumentRepository documentRepository)
        {
            _conventionId = conventionId;
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));            
            _placesQueries = placesQueries ?? throw new ArgumentNullException(nameof(placesQueries));
            _contactQueries = contactQueries ?? throw new ArgumentNullException(nameof(contactQueries));
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _typeConvention = typeConvention;
            _numeroConvention = numeroConvention;
            _sessionResult = sessionResult ?? throw new ArgumentNullException(nameof(sessionResult));

            ChooseDocumentCommand = new RelayCommand(ExecuteChooseDocumentAsync);
            PrintCommand = new RelayCommand(ExecutePrintCommand);
        }
        
        public override async Task Init()
        {
            var placesTask = Task.Run(()=>_placesQueries.GetConventionPlaces(_conventionId));
            var contactTask = LoadContact(_conventionId);

            await Task.WhenAll(placesTask, contactTask);

            Places = new ObservableCollection<IConventionPlaceResult>(placesTask.Result);
        }

        private async Task LoadContact(Guid conventionId)
        {
            var contact = await Task.Run(() => _contactQueries.GetConventionContact(conventionId));
            if (contact != null)
            {
                Nom = contact.Nom;
                Prenom = contact.Prenom;
                Email = contact.Email;
                Telephone = contact.Telephone;
            }
        }

        public ObservableCollection<IConventionPlaceResult> Places
        {
            get => _places;
            set { Set(()=>Places, ref _places, value); }
        }

        public string Nom
        {
            get => _nom;
            set { Set(() => Nom, ref _nom, value); }
        }

        public string Prenom
        {
            get => _prenom;
            set { Set(()=>Prenom, ref _prenom, value); }
        }

        public string Email
        {
            get => _email;
            set { Set(()=>Email, ref _email, value); }
        }

        public string Telephone
        {
            get => _telephone;
            set { Set(()=>Telephone, ref _telephone, value); }
        }

        public string DocumentPath
        {
            get => _documentPath;
            set { Set(()=>DocumentPath, ref _documentPath, value); }
        }

        public RelayCommand ChooseDocumentCommand { get; }
        private void ExecuteChooseDocumentAsync()
        {
            var openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Pdf|*.pdf|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;
          
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                DocumentPath = openFileDialog1.FileName;
        }

        public RelayCommand PrintCommand { get; }
        private void ExecutePrintCommand()
        {
            HandleMessageBoxError.Execute(()=>{
                var firstPlace = Places.First();
                string doc;
                if(_typeConvention == TypeConvention.Gratuite)
                    doc = _documentRepository.CreateConventionGratuite(_numeroConvention, firstPlace.Societe, firstPlace.Adresse, firstPlace.CodePostal, firstPlace.Ville, new NomComplet(Nom, Prenom), _sessionResult.Formation, _sessionResult.DateDebut, _sessionResult.Dur�e, _sessionResult.Lieu, Places.Select(a=>new Participant(a.Stagiaire, a.Societe)).ToList());
                else
                    doc = _documentRepository.CreateConventionPayante(_numeroConvention, firstPlace.Societe, firstPlace.Adresse, firstPlace.CodePostal, firstPlace.Ville, new NomComplet(Nom, Prenom), _sessionResult.Formation, _sessionResult.DateDebut, _sessionResult.Dur�e, _sessionResult.Lieu, Places.Select(a => new Participant(a.Stagiaire, a.Societe)).ToList());

                Process.Start(doc);
            });
        }

        protected override async Task ExecuteValiderAsync()
        {
            if (!File.Exists(DocumentPath))
            {
                MessageBox.Show("Le fichier s�lectionn� est introuvable", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await HandleMessageBoxError.ExecuteAsync(async () => { 
                var documentId = await Task.Run(()=>_documentRepository.SaveConvention(DocumentPath));
                await Task.Run(()=>_applicationService.Command<SignConvention>().Execute(_conventionId, documentId));
                await base.ExecuteValiderAsync();
            });

        }             
    }
}