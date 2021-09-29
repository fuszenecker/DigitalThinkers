import { useEffect, useState } from 'react';
import './App.css';

import { useInjection } from 'inversify-react';
import Backend from './services/Backend';
import { Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material';

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

  console.log("Coins: " + JSON.stringify(coinsState))
  const rows = []

  for (const key in coinsState) {
    if (coinsState.hasOwnProperty(key)) {
      const element: any = coinsState[key];
      
      console.log(`${key} --> ${element}`)
      rows.push(
        <TableRow key={key}>
          <TableCell component="th" scope="row">
            {key}
          </TableCell>
          <TableCell align="right">{element}</TableCell>
        </TableRow>
      )
    }
  }  

  return (
    <TableContainer component={Paper}>
    <Table sx={{ minWidth: 650 }} aria-label="simple table">
      <TableHead>
        <TableRow>
          <TableCell>Coin/banknote</TableCell>
          <TableCell align="right">Number of items</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {rows}
      </TableBody>
    </Table>
  </TableContainer>
  );
}

export default App;
