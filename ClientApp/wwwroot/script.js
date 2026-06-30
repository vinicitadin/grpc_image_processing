const imageInput = document.getElementById('imageInput');
const previewImage = document.getElementById('previewImage');
const previewCard = document.getElementById('previewCard');
const sendButton = document.getElementById('sendButton');
const downloadLink = document.getElementById('downloadLink');
const statusMessage = document.getElementById('statusMessage');

imageInput.addEventListener('change', handleImageSelection);

function handleImageSelection() {
  const file = imageInput.files?.[0];
  if (!file) {
    previewImage.src = '';
    previewCard.classList.remove('has-image');
    sendButton.disabled = true;
    sendButton.classList.add('disabled');
    downloadLink.classList.add('disabled');
    statusMessage.textContent = 'Nenhuma imagem selecionada.';
    return;
  }

  const url = URL.createObjectURL(file);
  previewImage.src = url;
  previewCard.classList.add('has-image');
  sendButton.disabled = true;
  sendButton.classList.add('disabled');
  downloadLink.classList.add('disabled');
  statusMessage.textContent = `Imagem selecionada: ${file.name} — envio/baixar desabilitados (front-end apenas).`;
}

sendButton.addEventListener('click', () => {
  statusMessage.textContent = 'Envio desabilitado';
});

downloadLink.addEventListener('click', (event) => {
  event.preventDefault();
  statusMessage.textContent = 'Download desabilitado';
});
