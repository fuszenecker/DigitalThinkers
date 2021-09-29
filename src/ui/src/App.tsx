import React, { useEffect, useState } from 'react';
import './App.css';
import { useInjection } from 'inversify-react';
import Backend from './services/Backend';

function App() {
  const [coinsState, setCoinState] = useState({} as any)
  const [reload, setReload] = useState(true)
  const backend = useInjection(Backend)

  useEffect(() => {
    if (reload) {
      backend.getCoins().then(result => {
        setReload(false)
        setCoinState(result)
      })
    }
  })

  const elements = []
  console.log("Coins: " + JSON.stringify(coinsState))

  for (const key in coinsState) {
    if (coinsState.hasOwnProperty(key)) {
      const element: any = coinsState[key];
      
      console.log(`${key} --> ${element}`)
      elements.push(<div>{key}: {element}</div>)
    }
  }  

  return (
    <div className="App">
      {elements}
    </div>
  );
}

export default App;
